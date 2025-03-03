using Avalanche.CommandModel;
using Avalanche.Runner;

namespace Avalanche.Domain
{
    public class TestFacade
    {
        private readonly IEventStore _store;

        public TestFacade(IEventStore store)
        {
            _store = store;
        }

        public TestSettings Start(string name, string path)
        {
            var settings = GetTestSettings(path);

            Task.Factory.StartNew(() =>
                {
                    //TODO: nicht via singleton lösen
                    var data = TestResultsCollection.Instance.StartNew(Guid.NewGuid().ToString(), name);

                    data.Status = TestRunStatus.Running;

                    data.Settings = settings;

                    var loadtest = new LoadTest(new Runner.Logging.TestResultsFacory(data), data.TestId, _store);
                    data.StartTime = DateTime.Now;
                    data.Results = loadtest.Run(settings);

                    data.Status = TestRunStatus.Done;


                    //
                    // Give the collector some time to finish the work
                    Task.Delay(10000).Wait();


                    loadtest.End();

                    //
                    // write result to file
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return settings;
        }

        public TestSettings GetTestSettings(string path)
        {
            var reader = new YamlMap.YamlFileReader();
            var settings = reader.Read<TestSettings>(path);

            return settings;
        }
    }
}
