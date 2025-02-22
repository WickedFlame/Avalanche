using Avalanche.Runner;

namespace Avalanche.Domain
{
    public class TestFacade
    {
        public void Start(string name, string path)
        {
            Task.Factory.StartNew(() =>
                {

                    var data = TestResultsCollection.Instance.StartNew(name);

                    data.Status = TestRunStatus.Running;

                    var settings = GetTestSettings(path);
                    data.Settings = settings;



                    var loadtest = new LoadTest(data.LogEntries);
                    data.Results = loadtest.Run(settings);

                    data.Status = TestRunStatus.Done;


                    //
                    // Give the collector some time to finish the work
                    Task.Delay(10000).Wait();

                    data.LogEntries.End();

                    //
                    // write result to file
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public TestSettings GetTestSettings(string path)
        {
            var reader = new YamlMap.YamlFileReader();
            var settings = reader.Read<TestSettings>(path);

            return settings;
        }
    }
}
