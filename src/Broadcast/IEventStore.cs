
namespace Broadcast
{
    public interface IEventStore
    {
        string Add<T>(string testId, DateTime time, T model) where T : IEvent;
    }
}
