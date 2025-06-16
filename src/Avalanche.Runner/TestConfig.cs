namespace Avalanche.Runner
{
    public class TestConfig
    {
        public List<string> Urls { get; set; }

        public string Request { get; set; }

        public string Name { get; set; }

        public int Iterations { get; set; }

        public int Threads { get; set; }

        public int Duration { get; set; }

        public int Interval { get; set; }

        /// <summary>
        /// Rampuptime in seconds
        /// </summary>
        public int RampupTime { get; set; }

        public bool UseCookies { get; set; } = true;

        public int Delay { get; set; }

        public InitConfig Init { get; set; }
    }

    public class InitConfig
    {
        public string Url { get; set; }
    }
}
