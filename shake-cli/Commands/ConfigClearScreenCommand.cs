using Serilog;
using Shake.Core.Utils;
using System.CommandLine;

namespace Shake.CLI.Commands;
internal class ConfigClearScreenCommand
{
    public static Command GetCommand(ILogger logger)
    {
        var clearScreenCmd = new Command("clear-screen", description: "Clear any screen settings from config");

        clearScreenCmd.SetHandler(() =>
        {
            var config = ConfigUtils.GetConfig();
            config.VideoShake = null;
            config.Save();
        });

        return clearScreenCmd;
    }
}
