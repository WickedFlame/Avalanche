namespace Avalanche.Runner
{
    public class TestCaseConfig
    {
        /// <summary>
        /// Amount of iterations the testcase is run for
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// Amount of users that run the testcase
        /// </summary>
        public int Users { get; set; }

        /// <summary>
        /// The duration that the testcase is run for
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// The interval each user runs a test for
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Rampuptime in seconds
        /// </summary>
        public int RampupTime { get; set; }

        /// <summary>
        /// Reuse the cookie
        /// </summary>
        public bool UseCookies { get; set; } = true;

        /// <summary>
        /// The delay between the tests
        /// </summary>
        public int Delay { get; set; }
    }
}
