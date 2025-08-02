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
CREATE TABLE IF NOT EXISTS Events (
  Id VARCHAR(255),
  TestId VARCHAR(255),
  Time DATE,
  EventType VARCHAR(500),
  Value VARCHAR (2000)
);
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
