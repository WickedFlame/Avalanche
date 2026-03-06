using Avalanche.DataSource;
using Broadcast;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System.Reflection;
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

        public async Task<AppendResult> AddAsync<T>(string eventId, string streamId, int streamVersion, string type, DateTime time, T data) where T : class
        {
            var value = JsonSerializer.Serialize(data);

            try
            {
                await Write(eventId, streamId, streamVersion, type, time, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Write to EventStora caused an error. Will Retry to add onece more.");

                //
                // recreate the connection and try again
                await Write(eventId, streamId, streamVersion, type, time, value);
            }

            return new AppendResult
            {
                Success = true,
                EventId = eventId,
                StreamId = streamId
            };
        }

        public Task<IEnumerable<EventEnvelope>> ReadStreamAsync(string streamId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EventEnvelope>> ReadAllAsync()
        {
            var db = _builder.Build();
            var events = await db.Query(nameof(Avalanche.DataSource.DTO.Events))
                .Select()
                .GetAsync<EventModel>();

            return events.Select(e => new EventEnvelope
            {
                Id = e.Id,
                StreamId = e.StreamId,
                StreamVersion = e.StreamVersion,
                Type = e.EventType,
                Time = e.Time,
                Data = CreateInstance(e.EventType, e.Data)
            });
        }

        private Task Write(string eventId, string streamId, int streamVersion, string type, DateTime time, string data)
        {
            var db = _builder.Build();
            return db.Query(nameof(Avalanche.DataSource.DTO.Events))
                    .InsertAsync(new EventModel
                    {
                        Id = eventId,
                        StreamId = streamId,
                        StreamVersion = streamVersion,
                        EventType = type,
                        Time = time,
                        Data = data
                    });
        }

        private static object CreateInstance(string eventType, string json)
        {
            var type = Type.GetType(eventType);
            var evnt = JsonSerializer.Deserialize(json, type);
            return evnt;
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
