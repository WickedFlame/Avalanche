using Avalanche.Domain;
using Avalanche.WriteModel;
using Avalanche.WriteModel.CommandHandlers;
using Avalanche.WriteModel.Commands;
using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Memory;
using Avalanche.WriteModel.Memory.EventHandlers;
using Broadcast;
using CommandLine;
using RestSharp;

namespace Avalanche
{
    [Verb("loadtest", true, HelpText = "Start a load test")]
    public class LoadTestOption
    {
        [Option('f', "configfile", HelpText = "UNC Path to the Configfile", Required = false)]
        public string ConfigFile { get; set; }

        [Option('u', "url", HelpText = "Url to the Avalanche server", Required = false)]
        public string Url { get; set; }

        public void Execute()
        {
            if (string.IsNullOrEmpty(ConfigFile))
            {
                Console.WriteLine("The Parameter --configfile or -f has to be provided with the path to the config file");
                ConfigFile = "LoadTest";
            }

            Console.WriteLine($"Start LoadTest from {ConfigFile}");

#if DEBUG
            // in debug wait until the website is started
            Console.WriteLine($"Wait until the website is started");
            System.Threading.Tasks.Task.Delay(10000).Wait();
#endif

            if (string.IsNullOrEmpty(Url))
            {
                //Url = "https://localhost:32773";
                Url = "https://host.docker.internal:32773";
            }

            var options = new RestClientOptions(Url)
            {
                FollowRedirects = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
            };



            var store = new InMemoryEventStore();

            var eventBus = new EventBus(store);
            if (!string.IsNullOrEmpty(Url))
            {
                eventBus.Subscribe<StartTestEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.TestRunEventHandler(new RestClient(options)));
                eventBus.Subscribe<EndTestEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.TestRunEventHandler(new RestClient(options)));
                eventBus.Subscribe<ThreadSummaryEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.SummaryEventHandler(new RestClient(options)));
                eventBus.Subscribe<TestSummaryEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.SummaryEventHandler(new RestClient(options)));
                eventBus.Subscribe<RampupEvent>(new RampupEventHandler());
                eventBus.Subscribe<RampdownEvent>(new RampupEventHandler());
                eventBus.Subscribe<IterationLogEvent>(new Avalanche.WriteModel.RestClient.EventHandlers.IterationEventHandler(new RestClient(options)));
            }
            else
            {
                eventBus.Subscribe<StartTestEvent>(new TestRunEventHandler());
                eventBus.Subscribe<EndTestEvent>(new TestRunEventHandler());
                eventBus.Subscribe<ThreadSummaryEvent>(new SummaryEventHandler());
                eventBus.Subscribe<TestSummaryEvent>(new SummaryEventHandler());
                eventBus.Subscribe<RampupEvent>(new RampupEventHandler());
                eventBus.Subscribe<RampdownEvent>(new RampupEventHandler());
                eventBus.Subscribe<IterationLogEvent>(new IterationEventHandler());
            }

            var dispatcher = new CommandDispatcher(eventBus);

            // LoadTest
            var path = $"testfiles/{ConfigFile}.yml";

            var facade = new TestFacade(dispatcher);
            facade.Start(ConfigFile, path);
        }
    }
}
