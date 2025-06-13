using Avalanche.WriteModel.Events;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sqlite.EventHandlers
{
    public class DeleteTestRunEventHandler :
        IEventHandler<DeleteTestRunEvent>
    {
        private readonly SQLiteConnection _connection;

        public DeleteTestRunEventHandler()
        {
            _connection = new SQLiteConnection(Constants.ReadModelDatabase);
            _connection.Open();
        }


        public void Handle(DeleteTestRunEvent @event)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM SummaryEvents where TestId = @testId";
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.ExecuteNonQuery();
            }

            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM IterationEvents where TestId = @testId";
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.ExecuteNonQuery();
            }

            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM TestRunDetail where TestId = @testId";
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.ExecuteNonQuery();
            }

            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM RampupEvents where TestId = @testId";
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
                cmd.ExecuteNonQuery();
            }

            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM TestRun where TestId = @testId";
                cmd.Parameters.Add(new SQLiteParameter("@testId", @event.TestId));
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
