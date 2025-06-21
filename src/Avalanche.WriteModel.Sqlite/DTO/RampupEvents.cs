namespace Avalanche.WriteModel.Sqlite.DTO
{
    public class RampupEvents
    {
        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string Name { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public bool IsWarmup { get; set; }

        public int ThreadId { get; set; }

        public int Value { get; set; }
    }
}
