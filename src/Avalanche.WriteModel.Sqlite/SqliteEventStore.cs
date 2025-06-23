using Avalanche.DataSource;
using Broadcast;
using SqlKata.Execution;
using System.Text.Json;

namespace Avalanche.WriteModel.Sqlite
{
    public class SqliteEventStore : IEventStore
    {
        private readonly IEventStoreConnectionBuilder _builder;

        public SqliteEventStore(IEventStoreConnectionBuilder connectionBuilder)
        {
            _builder = connectionBuilder;
        }

        public string Add<T>(string testId, DateTime time, T model) where T : IEvent
        {
            var id = Guid.NewGuid().ToString();
            var value = JsonSerializer.Serialize(model);
            var type = model.GetType().AssemblyQualifiedName;

            try
            {
                Write(id, testId, time, type, value);
            }
            catch (Exception ex)
            {
                //
                // recreate the connection and try again
                Write(id, testId, time, type, value);
            }

            return id;
        }

        private void Write(string id, string testId, DateTime time, string type, string value)
        {
            var db = _builder.Build();
            db.Query(nameof(Avalanche.DataSource.DTO.Events))
                    .Insert(new
                    {
                        Id = id,
                        TestId = testId,
                        Time = time,
                        EventType = type,
                        Value = value
                    });
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
