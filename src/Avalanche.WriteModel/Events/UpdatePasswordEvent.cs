using Broadcast;

namespace Avalanche.WriteModel.Events
{
    public class UpdatePasswordEvent
    {
        public string UserId { get; set; }

        public string Password { get; set; }
    }
}
