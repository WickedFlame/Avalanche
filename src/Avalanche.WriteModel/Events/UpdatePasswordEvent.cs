using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class UpdatePasswordEvent : IEvent
    {
        public string UserId { get; set; }

        public string Password { get; set; }
    }
}
