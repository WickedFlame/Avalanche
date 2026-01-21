
namespace Avalanche.WriteModel.Sql
{
    public class EventModel
    {
        public string Id { get; set; }

        public string StreamId { get; set; }

        public int StreamVersion { get; set; }

        public DateTime Time { get; set; }

        public string EventType { get; set; }

        public string Data { get; set; }
    }
}
