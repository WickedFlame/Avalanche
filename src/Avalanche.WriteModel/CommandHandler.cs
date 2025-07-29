namespace Avalanche.WriteModel
{
    public abstract class CommandHandler<T> : ICommandHandler<T> where T : class, ICommand
    {
        public void Handle(ICommand @event)
        {
            if (@event is T evnt)
            {
                Handle(evnt);
            }
        }

        public abstract void Handle(T @event);

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
