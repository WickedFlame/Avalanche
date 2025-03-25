using Avalanche.WriteModel.Events;
using System.Data.SQLite;

namespace Avalanche.WriteModel.EventHandlers
{
    public class EndThreadEventHandler : IEventHandler<EndLogEvent>
    {
        private readonly SQLiteConnection _connection;

        public EndThreadEventHandler()
        {
            _connection = new SQLiteConnection("Data Source=readmodel.db");
            _connection.Open();
        }

        public void Handle(EndLogEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO RampupEvents (Id, TestId, Name, Time, ThreadId, Value) Values (@id, @testId, @name, @time, @threadId, @value)";

                cmd.Parameters.Add(new SQLiteParameter("@id", Guid.NewGuid().ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@name", @event.Name));
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@time", @event.Time));
                cmd.Parameters.Add(new SQLiteParameter("@threadId", @event.Thread));
                cmd.Parameters.Add(new SQLiteParameter("@value", -1));

                cmd.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // do stuf here
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
