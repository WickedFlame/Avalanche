using Avalanche.Runner;

namespace Avalanche.Domain
{
    public class TestFacade
    {
        public void Start(string name, string path)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {

                var data = TestResultsCollection.Instance.StartNew(name);

                data.Status = TestRunStatus.Running;

                var settings = GetTestSettings(path);
                data.Settings = settings;



                var loadtest = new LoadTest(data.LogEntries);
                data.Results = loadtest.Run(settings);

                data.Status = TestRunStatus.Done;

                //
                // write result to file
            });
        }

        public TestSettings GetTestSettings(string path)
        {
            var reader = new YamlMap.YamlFileReader();
            var settings = reader.Read<TestSettings>(path);

            return settings;
        }
    }
}
