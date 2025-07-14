using System.Data.SQLite;

namespace Avalanche.DataSource.Sqlite
{
    public static class EventStoreBuilder
    {
        public static void CreateEventStore()
        {
            const string _query = @"
CREATE TABLE IF NOT EXISTS Events (
  Id VARCHAR(255),
  TestId VARCHAR(255),
  Time DATETIME,
  EventType VARCHAR(500),
  Value VARCHAR (2000)
);
";
            using (var connection = new SQLiteConnection(Constants.EventStoreDatabase))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _query;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void CreateWriteModel()
        {
            const string _query = @"
CREATE TABLE IF NOT EXISTS TestRun (
  TestId VARCHAR(255),
  Scenario VARCHAR(255),
  StartTime DATETIME,
  EndTime DATETIME,
  Status VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS RampupEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  TestCase VARCHAR(255),
  Time DATETIME,
  ThreadId VARCHAR(255),
  Value INT
);

CREATE TABLE IF NOT EXISTS TestRunDetail (
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  TestCase VARCHAR(255),
  ThreadId VARCHAR(255),
  Throughput REAL,
  Iterations BIGINT
);

CREATE TABLE IF NOT EXISTS IterationEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  Time DATETIME,
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
  Time DATETIME,
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
            using (var connection = new SQLiteConnection(Constants.ReadModelDatabase))
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
