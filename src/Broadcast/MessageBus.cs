namespace Broadcast
{
    public class MessageBus : IMessageBus
    {
        private readonly Dictionary<Type, IMessageHandler> _handlers = [];

        public void Register<Tevent>(IMessageHandler<Tevent> handler)
        {
            _handlers[typeof(Tevent)] = handler;
        }

        public virtual void Send<Tevent>(Tevent @event)
        {
            var handler = _handlers[@event.GetType()] as IMessageHandler<Tevent>;
            if (handler == null)
            {
                return;
            }

            handler.Handle(@event);
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
}
