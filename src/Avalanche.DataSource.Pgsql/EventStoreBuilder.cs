using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Avalanche.DataSource.Pgsql
{
    public class EventStoreBuilder : IDataStoreBuilder
    {
        private readonly IConfiguration _config;

        public EventStoreBuilder(IConfiguration config)
        {
            _config = config;
        }

        public void CreateEventStore()
        {
            const string _query = @"
CREATE TABLE IF NOT EXISTS public.events (
  id             text        NOT NULL UNIQUE,
  streamid       text        NOT NULL,
  streamversion  integer     NOT NULL,
  eventtype      text        NOT NULL,
  time           timestamptz NOT NULL DEFAULT now(),
  data           text        NOT NULL
);


-- Migration: Events legacy -> new schema
-- Legacy schema:
--   Id varchar(255), TestId varchar(255), Time date, EventType varchar(500), Value varchar(2000)
-- New schema:
--   Id text not null unique, StreamId text not null, StreamVersion int not null,
--   EventType text not null, Time timestamptz not null default now(), Data text not null
--
-- Indicator that migration is needed: column ""testid"" exists on public.events

BEGIN;

DO $$
DECLARE
  needs_migration boolean;
BEGIN
  SELECT EXISTS (
    SELECT 1
    FROM information_schema.columns
    WHERE table_schema = 'public'
      AND table_name   = 'events'
      AND lower(column_name) = 'testid'
  )
  INTO needs_migration;

  IF NOT needs_migration THEN
    RAISE NOTICE 'Events table already in new schema (no ""TestId"" column). No changes applied.';
    RETURN;
  END IF;

  -- Prevent accidental reruns if backup already exists
  IF EXISTS (
    SELECT 1
    FROM information_schema.tables
    WHERE table_schema = 'public'
      AND table_name   = 'events_legacy_backup'
  ) THEN
    RAISE EXCEPTION 'Backup table public.events_legacy_backup already exists. Aborting to avoid overwriting.';
  END IF;

  -- Create the new table with the desired schema
  CREATE TABLE IF NOT EXISTS public.events_new (
    id             text        NOT NULL UNIQUE,
    streamid       text        NOT NULL,
    streamversion  integer     NOT NULL,
    eventtype      text        NOT NULL,
    time           timestamptz NOT NULL DEFAULT now(),
    data           text        NOT NULL
  );

  -- Helper: convert ""Value"" to text; if legacy column appears to be JSON, keep as-is, otherwise store as plain text.
  -- Also: convert DATE -> timestamptz (midnight UTC). If Time is NULL, use now().
  INSERT INTO public.events_new (id, streamid, streamversion, eventtype, time, data)
  SELECT
    e.id::text                                        AS id,
    e.testid::text                                    AS streamid,
    0                                                 AS streamversion,
    e.eventtype::text                                 AS eventtype,
    COALESCE((e.time::timestamp AT TIME ZONE 'UTC'), now()) AS time,
    COALESCE(e.value::text, 'null')                  AS data
  FROM public.events e;

  -- Optional: ensure row counts match (simple sanity check)
  IF (SELECT count(*) FROM public.events_new) <> (SELECT count(*) FROM public.events) THEN
    RAISE EXCEPTION 'Row count mismatch after copy (events_new vs events). Aborting.';
  END IF;

  -- Keep old table as backup
  ALTER TABLE public.events RENAME TO events_legacy_backup;

  -- Swap in the new table
  ALTER TABLE public.events_new RENAME TO events;

  RAISE NOTICE 'Migration completed. Old table kept as public.events_legacy_backup.';
END $$;

COMMIT;

-- If you are satisfied, you can later drop the backup:
-- DROP TABLE public.events_legacy_backup;

";
            CreateDatabaseIfNotExists("eventstore", _config);

            var builder = new ConnectionBuilder(Constants.EventStoreDatabase, _config);
            using (var connection = new NpgsqlConnection(builder.BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _query;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void CreateDatabaseIfNotExists(string db, IConfiguration config)
        {
            var builder = new ConnectionBuilder("postgres", config);
            using (var connection = new NpgsqlConnection(builder.BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM pg_database WHERE datname = '{db}';";
                    if (cmd.ExecuteReader().Read())
                    {
                        return;
                    }

                    cmd.Cancel();
                }
            }

            using (var connection = new NpgsqlConnection(builder.BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"CREATE DATABASE {db};";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CreateWriteModel()
        {
            const string _query = @"
CREATE TABLE IF NOT EXISTS TestRun (
  TestId VARCHAR(255)  PRIMARY KEY,
  Scenario VARCHAR(255),
  StartTime TIMESTAMP,
  EndTime TIMESTAMP,
  Status VARCHAR(100),
  Runner VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS RampupEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  TestCase VARCHAR(255),
  Time TIMESTAMP,
  ThreadId INT,
  Value INT
);

CREATE TABLE IF NOT EXISTS TestRunDetail (
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  TestCase VARCHAR(255),
  ThreadId INT,
  Throughput REAL,
  Iterations BIGINT,
  AverageMilliseconds REAL
);

CREATE TABLE IF NOT EXISTS IterationEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  Time TIMESTAMP,
  ThreadId INT,
  TestCase VARCHAR(255),
  Throughput REAL,
  AverageMilliseconds REAL,
  ContentLength BIGINT,
  IsWarmup BOOLEAN,
  Message VARCHAR(500),
  StatusCode VARCHAR(50),
  Error BOOLEAN
);

CREATE TABLE IF NOT EXISTS SummaryEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  Time TIMESTAMP,
  TestCase VARCHAR(255),
  Type VARCHAR(100),
  ThreadId INT,
  Iterations INT,
  AverageMilliseconds REAL,
  TotalMilliseconds REAL,
  Throughput REAL,
  Slowest REAL,
  Fastest REAL
);

CREATE TABLE IF NOT EXISTS Users (
  Id VARCHAR(255) PRIMARY KEY,
  Username VARCHAR(255),
  Password VARCHAR(500),
  Name VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS Roles (
  Id VARCHAR(255) PRIMARY KEY,
  Name VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS UserRoles (
  UserId VARCHAR(255) REFERENCES Users(Id) ON DELETE CASCADE,
  RoleId VARCHAR(255) REFERENCES Roles(Id) ON DELETE CASCADE
);

INSERT INTO Roles (Id, Name) VALUES ('A42BCC71-20E6-44A5-9F47-45936A1877FE', 'Admin') ON CONFLICT DO NOTHING;

CREATE TABLE IF NOT EXISTS ApiKeys (
  Name VARCHAR(255) PRIMARY KEY,
  Value VARCHAR(500),
  Created TIMESTAMP,
  Expires TIMESTAMP
);

";
            CreateDatabaseIfNotExists("readmodel", _config);

            var builder = new ConnectionBuilder(Constants.ReadModelDatabase, _config);
            using (var connection = new NpgsqlConnection(builder.BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _query;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RecreateWriteModel()
        {
            var builder = new ConnectionBuilder("postgres", _config);
            using (var connection = new NpgsqlConnection(builder.BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"DROP DATABASE readmodel WITH (FORCE);";
                    cmd.ExecuteNonQuery();
                }
            }

            NpgsqlConnection.ClearAllPools();

            CreateWriteModel();
        }
    }
}
