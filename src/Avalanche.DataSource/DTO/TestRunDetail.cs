namespace Avalanche.DataSource.DTO
{
    public class TestRunDetail
    {
        public string TestId { get; set; }

        public string ThreadId { get; set; }

        public double Throughput { get; set; }

        public int Iterations {  get; set; }
    }
}
