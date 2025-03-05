using Avalanche.WriteModel.Events;
using System.Data.SQLite;

namespace Avalanche.WriteModel.EventHandlers
{
    public class EndTestEventHandler : IEventHandler<EndTestEvent>
    {
        private readonly SQLiteConnection _connection;

        public EndTestEventHandler()
        {
            _connection = new SQLiteConnection("Data Source=readmodel.db");
            _connection.Open();
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
            }
        }
    }
}
