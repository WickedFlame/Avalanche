
namespace Avalanche.ReadModel.Queries
{
    public class GetTestRun : IQuery
    {
        public string TestId { get; set; }

        public string TestName { get; set; }
    }
}
