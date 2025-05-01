using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite;
using System.Data;
using System.Data.SQLite;

namespace Avalanche.WriteModel.EventHandlers
{
    public class SummaryEventHandler :
        IEventHandler<ThreadSummaryEvent>,
        IEventHandler<TestSummaryEvent>
    {
        private readonly SQLiteConnection _connection;

        public SummaryEventHandler()
        {
            _connection = new SQLiteConnection(Constants.ReadModelDatabase);
            _connection.Open();
        }

        public void Handle(ThreadSummaryEvent @event)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO SummaryEvents (Id, TestId, TestCase, Type, ThreadNumber, Iterations, AverageTicks, TotalTime, Fastest, Slowest, Increase, InitialSize, EndSize, Throughput) Values (@id, @testId, @testcase, @type, @threadNumber, @iterations, @avgTicks, @totalTime, @fastest, @slowest, @increase, @initialSize, @endSize, @throughput)";

                cmd.Parameters.Add(new SQLiteParameter("@id", Guid.NewGuid().ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@testcase", @event.TestCase));
                cmd.Parameters.Add(new SQLiteParameter("@type", "ThreadSummary"));
                cmd.Parameters.Add(new SQLiteParameter("@threadNumber", @event.ThreadNumber));
                cmd.Parameters.Add(new SQLiteParameter("@iterations", @event.Iterations));
                cmd.Parameters.Add(new SQLiteParameter("@avgTicks", @event.AverageTicks));
                cmd.Parameters.Add(new SQLiteParameter("@totalTime", @event.TotalTime));
                cmd.Parameters.Add(new SQLiteParameter("@fastest", @event.Fastest));
                cmd.Parameters.Add(new SQLiteParameter("@slowest", @event.Slowest));
                cmd.Parameters.Add(new SQLiteParameter("@increase", @event.Increase));
                cmd.Parameters.Add(new SQLiteParameter("@initialSize", @event.InitialSize));
                cmd.Parameters.Add(new SQLiteParameter("@endSize", @event.EndSize));
                cmd.Parameters.Add(new SQLiteParameter("@throughput", @event.Throughput));

                cmd.ExecuteNonQuery();
            }
        }

        public void Handle(TestSummaryEvent @event)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO SummaryEvents (Id, TestId, TestCase, Type, ThreadNumber, Iterations, AverageTicks, TotalTime, Fastest, Slowest, Increase, InitialSize, EndSize, Throughput) Values (@id, @testId, @testcase, @type, @threadNumber, @iterations, @avgTicks, @totalTime, @fastest, @slowest, @increase, @initialSize, @endSize, @throughput)";

                cmd.Parameters.Add(new SQLiteParameter("@id", Guid.NewGuid().ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@testcase", @event.TestCase));
                cmd.Parameters.Add(new SQLiteParameter("@type", "TestSummary"));
                cmd.Parameters.Add(new SQLiteParameter("threadNumber", DBNull.Value));
                cmd.Parameters.Add(new SQLiteParameter("@iterations", @event.Iterations));
                cmd.Parameters.Add(new SQLiteParameter("@avgTicks", @event.AverageTicks));
                cmd.Parameters.Add(new SQLiteParameter("@totalTime", @event.TotalTime));
                cmd.Parameters.Add(new SQLiteParameter("@fastest", @event.Fastest));
                cmd.Parameters.Add(new SQLiteParameter("@slowest", @event.Slowest));
                cmd.Parameters.Add(new SQLiteParameter("@increase", @event.Increase));
                cmd.Parameters.Add(new SQLiteParameter("@initialSize", @event.InitialSize));
                cmd.Parameters.Add(new SQLiteParameter("@endSize", @event.EndSize));
                cmd.Parameters.Add(new SQLiteParameter("@throughput", @event.Throughput));

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
