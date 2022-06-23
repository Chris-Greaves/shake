using Serilog;
using Shake.Core.Utils;
using System.CommandLine;

namespace Shake.CLI.Commands;
internal class AudioListCommand
{
    public static Command GetCommand(ILogger logger)
    {
        var listAudioCmd = new Command("list", description: "List audio devices to keep awake");

        listAudioCmd.SetHandler(() =>
        {
            foreach (var device in AudioUtils.GetAudioDevices())
            {
                Console.WriteLine(device);
            }
        });

        return listAudioCmd;
    }
}
