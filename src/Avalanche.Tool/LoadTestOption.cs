using Avalanche.Domain;
using Avalanche.WriteModel;
using Avalanche.WriteModel.CommandHandlers;
using Avalanche.WriteModel.Commands;
using Avalanche.WriteModel.Sqlite;
using Broadcast;
using CommandLine;

namespace Avalanche
{
    [Verb("loadtest", true, HelpText = "Start a load test")]
    public class LoadTestOption
    {
        [Option('f', "configfile", HelpText = "UNC Path to the Configfile", Required = false)]
        public string ConfigFile { get; set; }

        public void Execute()
        {
            if (string.IsNullOrEmpty(ConfigFile))
            {
                Console.WriteLine("The Parameter --configfile or -f has to be provided with the path to the config file");
                ConfigFile = "LoadTest";
            }

            Console.WriteLine($"Start LoadTest from {ConfigFile}");


            var store = new SqliteEventStore();
            var eventBus = new EventBus(store);
            var dispatcher = new Dispatcher<ICommand>();
            dispatcher.Register<StartTestCommand>(new StartTestCommandHandler(eventBus));
            dispatcher.Register<EndTestCommand>(new EndTestCommandHandler(eventBus));
            dispatcher.Register<TestResultCommand>(new TestResultCommandHandler(eventBus));

            dispatcher.Register<StartupThreadCommand>(new StartupThreadCommandHandler(eventBus));
            dispatcher.Register<EndThreadCommand>(new EndThreadCommandHandler(eventBus));
            dispatcher.Register<IterationCommand>(new IterationCommandHandler(eventBus));





            // LoadTest
            var path = $"testfiles/{ConfigFile}.yml";

            var facade = new TestFacade(dispatcher);
            facade.Start(ConfigFile, path);
        }
    }
}
