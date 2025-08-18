using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class DeleteUserEvent : IEvent
    {
        public string UserId { get; set; }
    }
}
