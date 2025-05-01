// See https://aka.ms/new-console-template for more information
using Avalanche;
using CommandLine;

string bot = @"

   ********   **      **   ********   **        ********   ***    ***   ********   ***    ***  ********
  ***    ***  **      **  ***    ***  **       ***    ***  ****   ***  ***    ***  ***    ***  ***     
  ***    ***   **    **   ***    ***  **       ***    ***  *****  ***  ***         ***    ***  ***     
  **********    **  **    **********  **       **********  *** ** ***  ***         **********  *******
  ***    ***     ****     ***    ***  **       ***    ***  ***  *****  ***         ***    ***  ***     
  ***    ***      **      ***    ***  **       ***    ***  ***   ****  ***    ***  ***    ***  ***     
  ***    ***      **      ***    ***  *******  ***    ***  ***    ***   ********   ***    ***  ********

";
Console.WriteLine(bot);

var arguments = Environment.GetCommandLineArgs()?.ToList();



//Opacc.Fof.Was.Cli.Extensions.OptionsRunner.Run(arguments);

CommandLine.Parser.Default.ParseArguments<LoadTestOption>(arguments)
    .MapResult((LoadTestOption opt) =>
        {
            opt.Execute();

            // 
            // delay the exit of the console
            // somehow some apps are ended too quickly and don't finish work
            System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(3)).Wait();

            return 1;
        },
        errs => 1);
