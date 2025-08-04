namespace Avalanche.Runner
{
    public class TestCase : TestCaseConfig
    {
        /// <summary>
        /// The urls that are run in each test
        /// </summary>
        public List<string> Urls { get; set; }

        /// <summary>
        /// The name of the testcase
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The config that is run for initialization
        /// </summary>
        public InitConfig Init { get; set; }
    }

    public class InitConfig
    {
        public string Url { get; set; }
    }
}
