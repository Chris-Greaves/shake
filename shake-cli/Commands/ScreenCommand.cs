using Serilog;
using Shake.Core.Utils;
using System.CommandLine;

namespace Shake.CLI.Commands;
internal class ScreenCommand
{
    public static Command GetCommand(ILogger logger)
    {
        var displayOpt = new Option<bool>("--display", getDefaultValue: () => false, description: "Switch to ensure the display also doesn't go to sleep");
        displayOpt.AddAlias("-d");
        var timerOpt = new Option<int>("--timer", getDefaultValue: () => 0, description: "Set a timer in seconds for the Shake to keep the PC awake");
        timerOpt.AddAlias("-t");

        var cmd = new Command("screen", description: "Keep PC and screen awake using Execuion States")
        {
            displayOpt,
            timerOpt
        };

        cmd.SetHandler(async (bool includeDisplay, int timer, CancellationToken token) =>
        {
            if (includeDisplay)
            {
                logger.Debug("User selected to keep display awake as well");
            }
            if (timer != 0)
            {
                if (timer < 0)
                {
                    logger.Error("Invalid value for --timer. Must be >= 0, actual value was {Timer}", timer);
                    return;
                }
                logger.Debug("Param: Timer set for {Timer} seconds", timer);
            }

            logger.Information("Video shake starting: {IncludeDisplay}, {Timer}", includeDisplay, timer);
            token.Register(() =>
            {
                logger.Information("Video shake stopping: {IncludeDisplay}, {Timer}", includeDisplay, timer);
            });
            await VideoUtils.SetupAwakeLoopAsync(includeDisplay, timer, token);
        }, displayOpt, timerOpt);

        return cmd;
    }
}
