using Avalanche.WriteModel.Events;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
{
    public class IterationEventHandler : IEventHandler<IterationLogEvent>
    {
        private readonly SQLiteConnection _connection;

        private readonly Dictionary<string, IterationElementsContainer> _events = new();

        public IterationEventHandler()
        {
            _connection = new SQLiteConnection(Constants.ReadModelDatabase);
            _connection.Open();
        }

        public void Handle(IterationLogEvent @event)
        {
            //using (var cmd = _connection.CreateCommand())
            //{
            //    cmd.CommandText = "INSERT INTO IterationEvents (Id, TestId, Time, ThreadId, Name, Message, RunNumber, IsWarmup, TotalMilliseconds) Values (@id, @testId, @time, @threadId, @name, @message, @runNumber, @isWarmup, @totalMilliseconds)";

            //    cmd.Parameters.Add(new SQLiteParameter("@id", Guid.NewGuid().ToString()));
            //    cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
            //    cmd.Parameters.Add(new SQLiteParameter("@time", @event.Time));
            //    cmd.Parameters.Add(new SQLiteParameter("@threadId", @event.Thread));
            //    cmd.Parameters.Add(new SQLiteParameter("@name", @event.Name));
            //    cmd.Parameters.Add(new SQLiteParameter("@message", @event.Message));
            //    cmd.Parameters.Add(new SQLiteParameter("@runNumber", @event.RunNumber));
            //    cmd.Parameters.Add(new SQLiteParameter("@isWarmup", @event.IsWarmup));
            //    cmd.Parameters.Add(new SQLiteParameter("@totalMilliseconds", @event.TotalMilliseconds));

            //    cmd.ExecuteNonQuery();
            //}

            var key = $"{@event.TestId}_{@event.Name}_{@event.Thread}";
            if (!_events.ContainsKey(key))
            {
                _events.Add(key, new IterationElementsContainer());
            }

            var lst = _events[key];
            lst.Add(@event);

            // only write to db if the last update was more than 2 seconds ago
            if (!lst.IsCheckValid())
            {
                return;
            }

            var items = lst.GetEvents();
            if (items.Count() < 10)
            {
                return;
            }

            var time = items.Last().Time - items.First().Time;
            var countPerSecond = items.Count() / time.TotalSeconds;

            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT 1 FROM TestRunDetail WHERE TestId = @testId AND TestCase = @testCase AND ThreadId = @threadId";
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@testCase", @event.Name));
                cmd.Parameters.Add(new SQLiteParameter("@threadId", @event.Thread));

                cmd.CommandText = cmd.ExecuteScalar() != null ?
                    "UPDATE TestRunDetail SET Throughput = @throughput WHERE TestId = @testId AND TestCase = @testCase AND ThreadId = @threadId" :
                    "INSERT INTO TestRunDetail (TestId, TestCase, ThreadId, Throughput) Values (@testId, @testCase, @threadId, @throughput)";

                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@testCase", @event.Name));
                cmd.Parameters.Add(new SQLiteParameter("@threadId", @event.Thread));
                cmd.Parameters.Add(new SQLiteParameter("@throughput", countPerSecond));
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

    public class IterationElementsContainer
    {
        private readonly List<IterationLogEvent> _events = new List<IterationLogEvent>();
        private DateTime _lastCheck;

        public void Add(IterationLogEvent @event)
        {
            _events.Add(@event);
        }

        public IEnumerable<IterationLogEvent> GetEvents()
        {
            return _events.Skip(Math.Max(0, _events.Count - 10)).OrderBy(e => e.Time).ToList();
        }

        public bool IsCheckValid()
        {
            var now = DateTime.Now;
            if ((now - _lastCheck).TotalSeconds < 2)
            {
                return false;
            }

            _lastCheck = now;
            return true;
        }
    }
}
