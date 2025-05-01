using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite;
using System.Data.SQLite;

namespace Avalanche.WriteModel.EventHandlers
{
    public class IterationEventHandler : IEventHandler<IterationLogEvent>
    {
        private readonly SQLiteConnection _connection;

        public IterationEventHandler()
        {
            _connection = new SQLiteConnection(Constants.ReadModelDatabase);
            _connection.Open();
        }

        public void Handle(IterationLogEvent @event)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO IterationEvents (Id, TestId, Time, ThreadId, Name, Message, RunNumber, IsWarmup, TotalMilliseconds) Values (@id, @testId, @time, @threadId, @name, @message, @runNumber, @isWarmup, @totalMilliseconds)";

                cmd.Parameters.Add(new SQLiteParameter("@id", Guid.NewGuid().ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@time", @event.Time));
                cmd.Parameters.Add(new SQLiteParameter("@threadId", @event.Thread));
                cmd.Parameters.Add(new SQLiteParameter("@name", @event.Name));
                cmd.Parameters.Add(new SQLiteParameter("@message", @event.Message));
                cmd.Parameters.Add(new SQLiteParameter("@runNumber", @event.RunNumber));
                cmd.Parameters.Add(new SQLiteParameter("@isWarmup", @event.IsWarmup));
                cmd.Parameters.Add(new SQLiteParameter("@totalMilliseconds", @event.TotalMilliseconds));

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
                // do stuf here;
            }
        }
    }
}
