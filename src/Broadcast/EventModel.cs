
namespace Broadcast
{
    public class EventModel
    {
        public string Id { get; set; }

        public string TestId { get; set; }

        public DateTime Time { get; set; }

        public Type EventType { get; set; }

        public string Value { get; set; }
    }
}
