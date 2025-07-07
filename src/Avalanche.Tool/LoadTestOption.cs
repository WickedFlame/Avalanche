using Avalanche.Domain;
using Avalanche.WriteModel;
using Avalanche.WriteModel.CommandHandlers;
using Avalanche.WriteModel.Commands;
using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Memory;
using Avalanche.WriteModel.Memory.EventHandlers;
using Broadcast;
using CommandLine;
using MeasureMap.Diagnostics;
using Microsoft.Extensions.Logging;
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

        public ILoggerFactory LoggerFactory { get; set; }

        public void Execute()
        {
            if (string.IsNullOrEmpty(ConfigFile))
            {
                Console.WriteLine("The Parameter --configfile or -f has to be provided with the path to the config file");

                ConfigFile = "LoadTest";
            }

            Console.WriteLine($"Scenario: {ConfigFile}");

#if DEBUG
            // in debug wait until the website is started
            //Logger.LogInformation($"Wait until the website is started");
            Console.WriteLine($"Wait until the website is started");
            System.Threading.Tasks.Task.Delay(10000).Wait();

            if (string.IsNullOrEmpty(Url))
            {
                //Url = "https://localhost:32773";
                Url = "https://host.docker.internal:32773";
            }
#endif

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
                // LoadTest
                var path = $"{ConfigFile}.yml";
                if(!File.Exists(path))
                {
                    path = $"scenarios/{ConfigFile}.yml";
                }

                var facade = new TestFacade(dispatcher, LoggerFactory);
                facade.Start(ConfigFile, path);

                //
                // Give the collector some time to finish the work
                Task.Delay(10000).Wait();
            }

            eventBus.Dispose();
        }
    }
}
