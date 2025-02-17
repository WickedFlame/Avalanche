using System;
using System.Linq;
using System.Text;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[NuGetPackageRequirement("Avalanche")]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.LoadTest);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [NuGetPackage("Opacc.Fof.Avalanche", "Opacc.Fof.Avalanche.dll")]
    readonly Tool Avalanche;

    [Parameter("")]
    readonly string ConfigFile;

    //
    // nuke loadtest -configfile "../tests/loadTest.yml"
    //

    Target LoadTest => _ => _
        .Executes(() =>
        {
            var parameters = new StringBuilder()
                .Append("loadtest");

            if(!string.IsNullOrEmpty(ConfigFile))
            {
                parameters.Append($" --configfile {ConfigFile}");
            }

            Avalanche(parameters.ToString());
        });

}
