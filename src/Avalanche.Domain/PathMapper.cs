namespace Avalanche.Domain
{
    public static class PathMapper
    {
        public static string GetScenarioPath()
        {
            var path = Environment.GetEnvironmentVariable("SCENARIO_PATH");
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                return path;
            }

            path = "../scenarios";
            if (Directory.Exists(path))
            {
                return path;
            }

            path = "./scenarios";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string GetScenarioFile(string scenario)
        {
            return $"{GetScenarioPath()}/{scenario}.yml";
        }
    }
}
