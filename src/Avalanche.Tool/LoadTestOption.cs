//using CommandLine;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MeasureMap;
//using System.Net;
//using System.Diagnostics;

namespace Avalanche
{
    //[Verb("loadtest", true, HelpText = "Start a load test")]
    //public class LoadTestOption : ICliOption
    //{
    //    [Option('f', "configfile", HelpText = "UNC Path to the Configfile", Required = false)]
    //    public string ConfigFile { get; set; } = "../../../../tests/LoadTest.yml";

    //    public void Execute()
    //    {
    //        if (string.IsNullOrEmpty(ConfigFile))
    //        {
    //            Console.WriteLine("The Parameter --configfile or -f has to be provided with the path to the config file");
    //            return;
    //        }

    //        Console.WriteLine($"Start LoadTest from {ConfigFile}");

    //        var reader = new YamlMap.YamlFileReader();
    //        var settings = reader.Read<TestSettings>(ConfigFile);

    //        var logger = LoggerConfiguration.Setup(lc =>
    //        {
    //            lc.AddWriter(new ConsoleLogWriter(category: "console"))
    //                .SetAsDefault();
    //        });

    //        var loadTest = new LoadTest(logger);
    //        loadTest.Run(settings);
    //    }
    //}
}
