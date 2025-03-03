using System.Data.Common;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Avalanche
{
    public static class EventStoreExtensions
    {
        public static void UseEventStore(this IApplicationBuilder app)
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
            using (var connection = new SQLiteConnection("Data Source=eventstore.db"))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _query;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UseReadModel(this IApplicationBuilder app)
        {
            const string _query = @"
CREATE TABLE IF NOT EXISTS TestRun (
  TestId VARCHAR(255),
  TestName VARCHAR(255),
  StartTime DATETIME
);

CREATE TABLE IF NOT EXISTS StartEvents (
  Id VARCHAR(255),
  TestId VARCHAR(255) REFERENCES TestRun(TestId) ON DELETE CASCADE,
  Time DATETIME,
  EventType VARCHAR(500),
  Value VARCHAR (2000)
);
";
            using (var connection = new SQLiteConnection("Data Source=readmodel.db"))
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
