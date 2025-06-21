namespace Avalanche.WriteModel.Sqlite.DTO
{
    public class TestRun
    {
        public string TestId { get; set; }

        public string Scenario { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Status { get; set; }
    }
}
