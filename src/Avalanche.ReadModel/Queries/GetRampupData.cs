namespace Avalanche.ReadModel.Queries
{
    public class GetRampupData : IQuery
    {
        public string TestId { get; set; }

        public string TestCase { get; set; }
    }
}
