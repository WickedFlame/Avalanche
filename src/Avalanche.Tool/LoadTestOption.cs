using Avalanche.Domain;
using Avalanche.Runner;
using Avalanche.WriteModel;
using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Memory;
using Avalanche.WriteModel.Memory.EventHandlers;
using Broadcast;
using CommandLine;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Diagnostics;
using System.Text;

namespace Avalanche
{
    [Verb("loadtest", true, HelpText = "Start a load test")]
    public class LoadTestOption
    {
        [Option('s', "scenario", HelpText = "UNC Path to the Scenario YML file. The filename has to be the same as the scenario", Required = false)]
        public string Scenario { get; set; }

        [Option('u', "url", HelpText = "Url to the Avalanche server", Required = false)]
        public string Url { get; set; }

        [Option('i', "input", HelpText = "Read the scenario from STDIN input", Required = false)]
        public bool Input { get; set; }

        public ILoggerFactory LoggerFactory { get; set; }

        public void Execute()
        {
            if (string.IsNullOrEmpty(Scenario))
            {
                Console.WriteLine("The Parameter --scenario or -s has to be provided with the path to the config file");

                Scenario = "LoadTest";
            }

#if DEBUG
            // in debug wait until the website is started
            System.Threading.Tasks.Task.Delay(10000).Wait();
#endif

            Console.WriteLine($"    AVALANCHE:");
            Console.WriteLine($"        * v{typeof(Program).Assembly.GetName().Version}");
            Console.WriteLine($"     SCENARIO:");
            Console.WriteLine($"        * {Scenario}");

            var store = new InMemoryEventStore();

            var eventBus = new EventBus(store);
            if (!string.IsNullOrEmpty(Url))
            {
                var options = new RestClientOptions(Url)
                {
                    FollowRedirects = true,
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                };

                eventBus.Subscribe<StartTestEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.TestRunEventHandler(new RestClient(options), LoggerFactory));
                eventBus.Subscribe<EndTestEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.TestRunEventHandler(new RestClient(options), LoggerFactory));
                eventBus.Subscribe<ThreadSummaryEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.SummaryEventHandler(new RestClient(options), LoggerFactory));
                eventBus.Subscribe<TestSummaryEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.SummaryEventHandler(new RestClient(options), LoggerFactory));
                eventBus.Subscribe<RampupEvent>(new RampupEventHandler());
                eventBus.Subscribe<RampdownEvent>(new RampupEventHandler());
                eventBus.Subscribe<IterationLogEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.IterationEventHandler(new RestClient(options), LoggerFactory));
                eventBus.Subscribe<IterationErrorEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.IterationEventHandler(new RestClient(options), LoggerFactory));
                eventBus.Subscribe<InitScenarioEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.ScenarioEventHandler(new RestClient(options), LoggerFactory));
            }

            eventBus.Subscribe<StartTestEvent>(new TestRunEventHandler());
            eventBus.Subscribe<ThreadSummaryEvent>(new SummaryEventHandler());
            eventBus.Subscribe<RampupEvent>(new RampupEventHandler());
            eventBus.Subscribe<RampdownEvent>(new RampupEventHandler());

            var outputHandler = new ConsoleOutpuEventHandler();
            eventBus.Subscribe<EndTestEvent>(outputHandler);
            eventBus.Subscribe<TestSummaryEvent>(outputHandler);
            eventBus.Subscribe<IterationLogEvent>(outputHandler);
            eventBus.Subscribe<IterationErrorEvent>(outputHandler);

            using (var dispatcher = new CommandDispatcher(eventBus))
            {
                var facade = new TestFacade(dispatcher, LoggerFactory);

                var scenario = ReadFromConsole(facade).GetAwaiter().GetResult();
                if (scenario == null || scenario.TestCases.Count == 0)
                {
                    // LoadTest
                    var path = GetFilePath(Scenario);
                    if (!System.IO.File.Exists(path))
                    {
                        var msg = new StringBuilder()
                            .AppendLine($"Could not find any configuration file for the Scenario {Scenario}");

                        Console.WriteLine(msg.ToString());

                        var scenarioPath = GetScenarioFolderPath();
                        if (System.IO.Directory.Exists(scenarioPath))
                        {
                            msg.AppendLine($"Possible scenarios:");
                            foreach (var file in System.IO.Directory.GetFiles(scenarioPath))
                            {
                                msg.AppendLine($"- {file}");
                            }
                        }

                        msg.Append("Test will be aborted.");

                        var logger = LoggerFactory.CreateLogger<LoadTestOption>();
                        logger.LogError(msg.ToString());
                        return;
                    }

                    scenario = facade.GetScenario(path);
                }

                TraceTestCases(scenario);

                eventBus.Send(new InitScenarioEvent
                {
                    Name = Scenario,
                    Description = scenario.Description,
                    TestCases = scenario.TestCases.Select(t => new WriteModel.Events.TestCase
                    {
                        Name = t.Name,
                        Urls = t.Urls,
                        Delay = t.Delay,
                        Duration = t.Duration,
                        Interval = t.Interval,
                        Iterations = t.Iterations,
                        RampupTime = t.RampupTime,
                        Users = t.Users,
                        UseCookies = t.UseCookies,
                        Init = t.Init != null ? new WriteModel.Events.InitConfig
                        {
                            Url = t.Init.Url
                        } : null
                    })
                });

                facade.Start(Scenario, scenario, new TestRunSettings { Runner = TestRunnerType.External });

                //
                // Give the collector some time to finish the work
                Task.Delay(5000).Wait();
            }

            eventBus.Dispose();
        }

        private async Task<Scenario> ReadFromConsole(TestFacade facade)
        {
            if (!Input)
            {
                return null;
            }

            Console.WriteLine("Reading the scenario from the STDIN");

            try
            {
                var data = await Console.In.ReadToEndAsync();
                if (!string.IsNullOrEmpty(data))
                {
                    return facade.Parse(data);
                }
            }
            catch
            {
                // just ignore
            }

            return null;
        }

        private static void TraceTestCases(Scenario scenario)
        {
            Console.WriteLine("    TESTCASES:");
            foreach (var tc in scenario.TestCases)
            {
                var sb = new StringBuilder()
                    .Append($"        * {tc.Name.PadRight(15)} Users: {tc.Users}");
                
                if (tc.Iterations > 0)
                {
                    sb.Append($", Iterations: {tc.Iterations}");
                }

                if (tc.Interval > 0)
                {
                    sb.Append($", Interval: {tc.Interval}");
                }

                if (tc.Duration > 0)
                {
                    sb.Append($", Duration: {tc.Duration}");
                }

                if (tc.Delay > 0)
                {
                    sb.Append($", Delay: {tc.Delay}");
                }

                Console.WriteLine(sb.ToString());
            }
        }

        private static string GetFilePath(string scenario)
        {
            var path = $"{scenario}.yml";
            if(File.Exists(path))
            {
                Debug.WriteLine($"Starting Scenario {scenario} in {path}");
                return path;
            }

            path = $"../{scenario}.yml";
            if (File.Exists(path))
            {
                Debug.WriteLine($"Starting Scenario {scenario} in {path}");
                return path;
            }

            path = $"scenarios/{scenario}.yml";
            if (File.Exists(path))
            {
                Debug.WriteLine($"Starting Scenario {scenario} in {path}");
                return path;
            }

            path = $"../scenarios/{scenario}.yml";
            if (File.Exists(path))
            {
                Debug.WriteLine($"Starting Scenario {scenario} in {path}");
                return path;
            }

            Debug.WriteLine($"File for Scenario {scenario} was not found");
            return $"{scenario}.yml";
        }

        private static string GetScenarioFolderPath()
        {
            var path = "scenarios";
            if (Directory.Exists(path))
            {
                return path;
            }

            path = "../scenarios";
            if (Directory.Exists(path))
            {
                return path;
            }

            return string.Empty;
        }
    }
}
