namespace Avalanche.Runner
{
    public class Scenario
    {
        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// The configuration that is used for each testcase if not overwritten in the testcase
        /// </summary>
        public TestCaseConfig Config { get; set; }

        /// <summary>
        /// Configuration for the authorization
        /// </summary>
        public Authorization Authorization { get; set; }

        /// <summary>
        /// The testcases in the scenario
        /// </summary>
        public List<TestCase> TestCases { get; set; } = new List<TestCase>();
    }
}
