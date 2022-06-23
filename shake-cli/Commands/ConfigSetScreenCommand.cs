using Serilog;
using Shake.Core.Utils;
using Shake.Core.Utils.ConfigModels;
using System.CommandLine;

namespace Shake.CLI.Commands;
internal class ConfigSetScreenCommand
{
    public static Command GetCommand(ILogger logger)
    {
        var displayOpt = new Option<bool>(new string[] { "--display", "-d" }, getDefaultValue: () => false, description: "Switch to ensure the display also doesn't go to sleep");
        var timerOpt = new Option<int>(new string[] { "--timer", "-t" }, getDefaultValue: () => 0, description: "Set a timer in seconds for the Shake to keep the PC awake");

        var setScreenCmd = new Command("set-screen", description: "Set screen shake in config")
        {
            displayOpt,
            timerOpt
        };

        setScreenCmd.SetHandler((bool includeDisplay, int timer) =>
        {
            var config = ConfigUtils.GetConfig();

            config.VideoShake = new VideoShake
            {
                IncludeDisplay = includeDisplay,
                Timer = timer
            };

            config.Save();
        }, displayOpt, timerOpt);

        return setScreenCmd;
    }
}
