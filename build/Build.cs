using System;
using System.IO;
using NuGet.Common;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.SonarScanner;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.SonarScanner.SonarScannerTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = Configuration.Release;

    [Solution] readonly Solution Solution;

    [Parameter("Version to be injected in the Build")]
    public string Version { get; set; } = "0.0.1";

    [Parameter("The Buildnumber provided by the CI")]
    public string BuildNo = $"{DateTime.Today.Month * 31 + DateTime.Today.Day}1";

    public AbsolutePath SourceDirectory => RootDirectory / "src";

    public AbsolutePath PublishDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            if (PublishDirectory.Exists())
            {
                PublishDirectory.GetDirectories().ForEach(x => x.DeleteDirectory());
            }

            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(d =>
            {
                try
                { 
                    d.DeleteDirectory();
                }
                catch { }
            });
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetVersion($"{Version}.{BuildNo}")
                .SetAssemblyVersion($"{Version}.{BuildNo}")
                .SetFileVersion($"{Version}.{BuildNo}")
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(true)
                .EnableNoRestore());
        });


    Target Deploy => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .Executes(() =>
        {
            // cleanup
            (PublishDirectory / "web").CreateOrCleanDirectory();

            DotNetPublish(o => o
                .SetConfiguration(Configuration)
                .SetVersion($"{Version}.{BuildNo}")
                .SetAssemblyVersion($"{Version}.{BuildNo}")
                .SetFileVersion($"{Version}.{BuildNo}")
                .SetProject(RootDirectory / "src" / "Avalanche")
                .SetPublishProfile("FolderProfile")
                .SetOutput(PublishDirectory / "web"));

            DotNetPublish(o => o
                .SetConfiguration(Configuration)
                .SetVersion($"{Version}.{BuildNo}")
                .SetAssemblyVersion($"{Version}.{BuildNo}")
                .SetFileVersion($"{Version}.{BuildNo}")
                .SetProject(RootDirectory / "src" / "Avalanche.Tool")
                .SetPublishProfile("FolderProfile")
                .SetOutput(PublishDirectory / "tool"));

            if((PublishDirectory / "web" / "scenarios").DirectoryExists())
            {
                Serilog.Log.Write(Serilog.Events.LogEventLevel.Information, $"Delete files in {(PublishDirectory / "web" / "scenarios")}");
                (PublishDirectory / "web" / "scenarios").GetFiles().ForEach(f => f.DeleteFile());
                (PublishDirectory / "web" / "scenarios").DeleteDirectory();
            }

            if ((PublishDirectory / "tool" / "scenarios").DirectoryExists())
            {
                Serilog.Log.Write(Serilog.Events.LogEventLevel.Information, $"Delete files in {(PublishDirectory / "tool" / "scenarios")}");
                (PublishDirectory / "tool" / "scenarios").GetFiles().ForEach(f => f.DeleteFile());
                (PublishDirectory / "tool" / "scenarios").DeleteDirectory();
            }
        });
}
