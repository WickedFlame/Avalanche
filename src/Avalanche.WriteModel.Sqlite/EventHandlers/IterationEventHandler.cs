using Avalanche.WriteModel.Events;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
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
                cmd.CommandText = "INSERT INTO IterationEvents (Id, TestId, Time, ThreadId, TestName, Throughput, AverageMilliseconds, IsWarmup) Values (@id, @testId, @time, @threadId, @name, @throughput, @averageMilliseconds, @isWarmup)";

                cmd.Parameters.Add(new SQLiteParameter("@id", Guid.NewGuid().ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@time", @event.Time));
                cmd.Parameters.Add(new SQLiteParameter("@threadId", @event.Thread));
                cmd.Parameters.Add(new SQLiteParameter("@name", @event.TestName));
                cmd.Parameters.Add(new SQLiteParameter("@averageMilliseconds", @event.AverageMilliseconds));
                cmd.Parameters.Add(new SQLiteParameter("@throughput", @event.Throughput));
                cmd.Parameters.Add(new SQLiteParameter("@isWarmup", @event.IsWarmup));

                cmd.ExecuteNonQuery();
            }

            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT 1 FROM TestRunDetail WHERE TestId = @testId AND TestCase = @testCase AND ThreadId = @threadId";
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@testCase", @event.TestName));
                cmd.Parameters.Add(new SQLiteParameter("@threadId", @event.Thread));

                cmd.CommandText = cmd.ExecuteScalar() != null ?
                    "UPDATE TestRunDetail SET Throughput = @throughput, Iterations = @iterations WHERE TestId = @testId AND TestCase = @testCase AND ThreadId = @threadId" :
                    "INSERT INTO TestRunDetail (TestId, TestCase, ThreadId, Throughput, Iterations) Values (@testId, @testCase, @threadId, @throughput, @iterations)";

                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@testCase", @event.TestName));
                cmd.Parameters.Add(new SQLiteParameter("@threadId", @event.Thread));
                cmd.Parameters.Add(new SQLiteParameter("@throughput", @event.Throughput));
                cmd.Parameters.Add(new SQLiteParameter("@iterations", @event.Iterations));
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
