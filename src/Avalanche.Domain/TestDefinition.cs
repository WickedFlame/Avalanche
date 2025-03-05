using Avalanche.Runner;

namespace Avalanche.Domain
{
    public class TestDefinition
    {
        public string Name { get; set; }

        public TestRunStatus State { get; set; }

        public DateTime Started { get; set; }
    }
}
