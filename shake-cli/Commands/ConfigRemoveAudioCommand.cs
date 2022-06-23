using Serilog;
using Shake.Core.Utils;
using System.CommandLine;

namespace Shake.CLI.Commands;
internal class ConfigRemoveAudioCommand
{
    public static Command GetCommand(ILogger logger)
    {
        var audioIdArg = new Argument<string>("Shake Id", "The Id of the Audio shake you want to delete");
        var removeAudioCmd = new Command("remove-audio", description: "Remove an audio shake")
        {
            audioIdArg
        };

        removeAudioCmd.SetHandler((string id) =>
        {
            var config = ConfigUtils.GetConfig();

            var toDelete = config.AudioShakes.SingleOrDefault(a => a.Id == id);

            if (toDelete is null)
            {
                return;
            }

            config.AudioShakes.Remove(toDelete);
            config.Save();
        }, audioIdArg);

        return removeAudioCmd;
    }
}
