using Npgsql;

namespace Avalanche.DataSource.Pgsql
{
    public static class EventStoreBuilder
    {
        public static void CreateEventStore()
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
            CreateDatabaseIfNotExists("eventstore");

            using (var connection = new NpgsqlConnection(Constants.EventStoreDatabase))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _query;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void CreateDatabaseIfNotExists(string db)
        {
            var cs = Constants.EventStoreDatabase.Replace($"Database={db};", "Database=postgres;");
            using (var connection = new NpgsqlConnection(cs))
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

            using (var connection = new NpgsqlConnection(cs))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"CREATE DATABASE {db};";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void CreateWriteModel()
        {
            const string _query = @"
CREATE TABLE IF NOT EXISTS TestRun (
  TestId VARCHAR(255)  PRIMARY KEY,
  Scenario VARCHAR(255),
  StartTime TIMESTAMP,
  EndTime TIMESTAMP,
  Status VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS RampupEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  Name VARCHAR(255),
  Time TIMESTAMP,
  ThreadId INT,
  Value INT
);

CREATE TABLE IF NOT EXISTS TestRunDetail (
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  TestCase VARCHAR(255),
  ThreadId INT,
  Throughput REAL,
  Iterations BIGINT
);

CREATE TABLE IF NOT EXISTS IterationEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  Time TIMESTAMP,
  ThreadId INT,
  TestName VARCHAR(255),
  Throughput REAL,
  AverageMilliseconds REAL,
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
  ThreadNumber VARCHAR(255),
  Iterations INT,
  AverageMilliseconds REAL,
  TotalMilliseconds REAL,
  Throughput REAL,
  Slowest REAL,
  Fastest REAL
);
";
            CreateDatabaseIfNotExists("readmodel");

            using (var connection = new NpgsqlConnection(Constants.ReadModelDatabase))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _query;

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
