using System.Data.SQLite;
using System.Text.Json;

namespace Avalanche.WriteModel
{
    public class EventStore : IEventStore
    {
        private readonly SQLiteConnection _connection;

        public EventStore()
        {
            _connection = new SQLiteConnection("Data Source=eventstore.db");
            _connection.Open();
        }

        public string Add<T>(string testId, DateTime time, T model) where T : IEvent
        {
            using (var cmd = _connection.CreateCommand())
            {
                var id = Guid.NewGuid().ToString();

                cmd.CommandText = "INSERT INTO Events (Id, TestId, Time, EventType, Value) Values (@id, @testId, @time, @eventType, @value)";
                cmd.Parameters.Add(new SQLiteParameter("@id", id));
                cmd.Parameters.Add(new SQLiteParameter("@testId", testId));
                cmd.Parameters.Add(new SQLiteParameter("@time", time));
                cmd.Parameters.Add(new SQLiteParameter("@eventType", model.GetType().AssemblyQualifiedName));
                cmd.Parameters.Add(new SQLiteParameter("@value", JsonSerializer.Serialize(model)));

                cmd.ExecuteNonQuery();

                return id;
            }
        }
    }
}
