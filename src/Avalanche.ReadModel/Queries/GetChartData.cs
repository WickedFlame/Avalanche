namespace Avalanche.ReadModel.Queries
{
    public class GetChartData : IQuery
    {
        public string TestId { get; set; }

        public object TestCase { get; set; }
    }
}
