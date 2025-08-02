using Microsoft.Extensions.Configuration;
using System;
using System.Data.SQLite;
using System.IO;
using System.Xml.Linq;

namespace Avalanche.DataSource.Sqlite
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
  Time DATETIME,
  EventType VARCHAR(500),
  Value VARCHAR (2000)
);
";
            var builder = new ConnectionStringBuilder(Constants.EventStore, _config);
            using (var connection = new SQLiteConnection(builder.BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _query;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CreateWriteModel()
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
  Iterations BIGINT,
  AverageMilliseconds REAL
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
            var builder = new ConnectionStringBuilder(Constants.ReadModel, _config);
            using (var connection = new SQLiteConnection(builder.BuildConnectionString()))
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
            var dataPath = ConnectionStringBuilder.GetDataPath(_config);
            var db = $"Data Source={dataPath}/{Constants.ReadModel}.db";
            if (File.Exists(db))
            {
                File.Delete(db);
            }

            CreateWriteModel();
        }
    }
}
