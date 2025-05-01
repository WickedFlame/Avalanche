using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sqlite;
using System.Data.SQLite;

namespace Avalanche.WriteModel.EventHandlers
{
    public class TestRunEventHandler :
        IEventHandler<StartTestEvent>,
        IEventHandler<EndTestEvent>
    {
        private readonly SQLiteConnection _connection;

        public TestRunEventHandler()
        {
            _connection = new SQLiteConnection(Constants.ReadModelDatabase);
            _connection.Open();
        }

        public void Handle(StartTestEvent evnt)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO TestRun (TestId, Scenario, StartTime, Status) Values (@testId, @scenario, @startTime, @status)";

                cmd.Parameters.Add(new SQLiteParameter("@testId", evnt.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@scenario", evnt.Scenario));
                cmd.Parameters.Add(new SQLiteParameter("@startTime", evnt.StartTime));
                cmd.Parameters.Add(new SQLiteParameter("@status", evnt.Status));

                cmd.ExecuteNonQuery();
            }
        }

        public void Handle(EndTestEvent evnt)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE TestRun SET EndTime = @endTime, Status = @status WHERE TestId = @testId";

                cmd.Parameters.Add(new SQLiteParameter("@testId", evnt.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@endTime", evnt.EndTime));
                cmd.Parameters.Add(new SQLiteParameter("@status", evnt.Status));

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
