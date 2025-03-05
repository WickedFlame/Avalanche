using Avalanche.WriteModel.Events;
using System.Data.SQLite;

namespace Avalanche.WriteModel.EventHandlers
{
    public class StartTestEventHandler : IEventHandler<StartTestEvent>
    {
        private readonly SQLiteConnection _connection;

        public StartTestEventHandler()
        {
            _connection = new SQLiteConnection("Data Source=readmodel.db");
            _connection.Open();
        }

        public void Handle(StartTestEvent evnt)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO TestRun (TestId, TestName, StartTime, Status) Values (@testId, @testName, @startTime, @status)";

                cmd.Parameters.Add(new SQLiteParameter("@testId", evnt.TestId));
                cmd.Parameters.Add(new SQLiteParameter("@testName", evnt.TestName));
                cmd.Parameters.Add(new SQLiteParameter("@startTime", evnt.StartTime));
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
            }
        }
    }
}
