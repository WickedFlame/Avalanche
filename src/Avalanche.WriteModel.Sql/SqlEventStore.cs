using Avalanche.DataSource;
using Broadcast;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System.Text.Json;

namespace Avalanche.WriteModel.Sql
{
    public class SqlEventStore : IEventStore
    {
        private readonly IEventStoreConnectionBuilder _builder;
        private readonly ILogger<SqlEventStore> _logger;

        public SqlEventStore(IEventStoreConnectionBuilder connectionBuilder, ILogger<SqlEventStore> logger)
        {
            _builder = connectionBuilder;
            _logger = logger;
        }

        public string Add<T>(string testId, DateTime time, T model) where T : class
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
                _logger.LogError(ex, "Write to EventStora caused an error. Will Retry to add onece more.");

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
