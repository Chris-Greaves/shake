using Serilog;
using Shake.Core.Utils;
using Shake.Core.Utils.ConfigModels;
using System.CommandLine;

namespace Shake.CLI.Commands;
internal class ConfigAddAudioCommand
{
    public static Command GetCommand(ILogger logger)
    {
        var deviceOpt = new Option<Guid>("--device", description: "The audio device to keep awake. For a list of devices with ids see 'shake audio list'");
        deviceOpt.AddAlias("-d");
        var delayOpt = new Option<int>("--delay", getDefaultValue: () => 5, description: "The delay between audio cues in seconds");
        var testOpt = new Option<bool>("--test", getDefaultValue: () => false, description: "Switch to use the audiable file to test it works");
        var addAudioCmd = new Command("add-audio", description: "Add new audio shake")
        {
            deviceOpt,
            delayOpt,
            testOpt
        };

        addAudioCmd.SetHandler((Guid device, int delay, bool useTestAudio) =>
        {
            var config = ConfigUtils.GetConfig();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var id = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            config.AudioShakes.Add(new AudioShake
            {
                Id = id,
                AudioDevice = device,
                Delay = delay,
                UseTestAudio = useTestAudio
            });

            config.Save();
        }, deviceOpt, delayOpt, testOpt);

        return addAudioCmd;
    }
}
