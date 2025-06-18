using Avalanche.WriteModel.Events;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
{
    public class SummaryEventHandler :
        IEventHandler<ThreadSummaryEvent>,
        IEventHandler<TestSummaryEvent>
    {
        private readonly object _lock = new object();
        private readonly SQLiteConnection _connection;

        public SummaryEventHandler()
        {
            _connection = new SQLiteConnection(Constants.ReadModelDatabase);
            _connection.Open();
        }

        public void Handle(ThreadSummaryEvent @event)
        {
            lock (_lock)
            {
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO SummaryEvents (Id, TestId, Time, TestCase, Type, ThreadNumber, Iterations, AverageMilliseconds, TotalMilliseconds, Throughput) Values (@id, @testId, @time, @testcase, @type, @threadNumber, @iterations, @avgMs, @totalMs, @throughput)";

                    cmd.Parameters.Add(new SQLiteParameter("@id", Guid.NewGuid().ToString()));
                    cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                    cmd.Parameters.Add(new SQLiteParameter("@time", DateTime.Now));
                    cmd.Parameters.Add(new SQLiteParameter("@testcase", @event.TestCase));
                    cmd.Parameters.Add(new SQLiteParameter("@type", "ThreadSummary"));
                    cmd.Parameters.Add(new SQLiteParameter("@threadNumber", @event.ThreadNumber));
                    cmd.Parameters.Add(new SQLiteParameter("@iterations", @event.Iterations));
                    cmd.Parameters.Add(new SQLiteParameter("@avgMs", @event.AverageMilliseconds));
                    cmd.Parameters.Add(new SQLiteParameter("@totalMs", @event.TotalMilliseconds));
                    cmd.Parameters.Add(new SQLiteParameter("@throughput", @event.Throughput));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Handle(TestSummaryEvent @event)
        {
            lock (_lock)
            {
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO SummaryEvents (Id, TestId, Time, TestCase, Type, ThreadNumber, Iterations, AverageMilliseconds, TotalMilliseconds, Throughput, Slowest, Fastest) Values (@id, @testId, @time, @testcase, @type, @threadNumber, @iterations, @avgMs, @totalMs, @throughput, @slowest, @fastest)";

                    cmd.Parameters.Add(new SQLiteParameter("@id", Guid.NewGuid().ToString()));
                    cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                    cmd.Parameters.Add(new SQLiteParameter("@time", DateTime.Now));
                    cmd.Parameters.Add(new SQLiteParameter("@testcase", @event.TestCase));
                    cmd.Parameters.Add(new SQLiteParameter("@type", "TestSummary"));
                    cmd.Parameters.Add(new SQLiteParameter("threadNumber", DBNull.Value));
                    cmd.Parameters.Add(new SQLiteParameter("@iterations", @event.Iterations));
                    cmd.Parameters.Add(new SQLiteParameter("@avgMs", @event.AverageMilliseconds));
                    cmd.Parameters.Add(new SQLiteParameter("@totalMs", @event.TotalMilliseconds));
                    cmd.Parameters.Add(new SQLiteParameter("@throughput", @event.Throughput));
                    cmd.Parameters.Add(new SQLiteParameter("@slowest", @event.Slowest));
                    cmd.Parameters.Add(new SQLiteParameter("@fastest", @event.Fastest));

                    cmd.ExecuteNonQuery();
                }
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
