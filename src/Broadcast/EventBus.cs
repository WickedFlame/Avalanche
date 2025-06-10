namespace Broadcast
{
    public class EventBus : IEventBus
    {
        private readonly List<MessageHandlerRegistration> _handlers = [];
        private readonly IEventStore _eventStore;

        public EventBus(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public void Subscribe<Tevent>(IMessageHandler<Tevent> handler)
        {
            _handlers.Add(new MessageHandlerRegistration(typeof(Tevent), handler));
        }

        public virtual void Send<Tevent>(Tevent @event)
        {
            var key = @event.GetType();
            if (!_handlers.Any(h => h.EventType == key))
            {
                Console.WriteLine($"No handler for event type {key}");
                return;
            }

            foreach (var registration in _handlers.Where(h => h.EventType == key))
            {
                var handler = registration.Handler as IMessageHandler<Tevent>;
                if (handler == null)
                {
                    return;
                }

                handler.Handle(@event);
            }
        }

        public void Publish<Tevent>(string id, DateTime time, Tevent @event) where Tevent : IEvent
        {
            _eventStore.Add(id, time, @event);
            Send(@event);
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
                // dispose here
            }
        }
    }

    public class MessageHandlerRegistration
    {
        public MessageHandlerRegistration(Type eventType, IMessageHandler handler)
        {
            EventType = eventType;
            Handler = handler;
        }

        public Type EventType { get; }

        public IMessageHandler Handler { get; }
    }
}
