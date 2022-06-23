using Serilog;
using Shake.Core.Utils;
using System.CommandLine;

namespace Shake.CLI.Commands;
internal class ConfigCommand
{
    public static Command GetCommand(ILogger logger)
    {
        var cmd = new Command("config", description: "Update the global config for shake.")
        {
            ConfigAddAudioCommand.GetCommand(logger.ForContext<ConfigAddAudioCommand>()),
            ConfigRemoveAudioCommand.GetCommand(logger.ForContext<ConfigRemoveAudioCommand>()),
            ConfigSetScreenCommand.GetCommand(logger.ForContext<ConfigSetScreenCommand>()),
            ConfigClearScreenCommand.GetCommand(logger.ForContext<ConfigClearScreenCommand>()),
            ConfigRunCommand.GetCommand(logger.ForContext<ConfigRunCommand>())
        };

        cmd.SetHandler(() =>
        {
            Console.Write(ConfigUtils.GetConfigAsString());
        });

        return cmd;
    }
}
