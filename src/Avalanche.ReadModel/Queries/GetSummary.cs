namespace Avalanche.ReadModel.Queries
{
    public class GetSummary : IQuery
    {
        public string TestId { get; set; }

        public string TestCase { get; set; }
    }
}
