using Serilog;
using Shake.Core.Utils;
using System.CommandLine;

namespace Shake.CLI.Commands;
internal class AudioCommand
{
    public static Command GetCommand(ILogger logger)
    {
        var deviceOpt = new Option<Guid>("--device", description: "The audio device to keep awake. For a list of devices with ids see 'shake audio list'");
        deviceOpt.AddAlias("-d");
        var delayOpt = new Option<int>("--delay", getDefaultValue: () => 5, description: "The delay between audio cues in seconds");
        var testOpt = new Option<bool>("--test", getDefaultValue: () => false, description: "Switch to use the audiable file to test it works");

        var audioCmd = new Command("audio", description: "Keep and audio device Awake by playing inaudible sounds on repeat")
        {
            deviceOpt,
            delayOpt,
            testOpt,
            AudioListCommand.GetCommand(logger.ForContext<AudioCommand>())
        };

        audioCmd.SetHandler(async (Guid device, int delay, bool test, CancellationToken token) =>
        {
            var deviceName = AudioUtils.GetAudioDeviceName(device);
            if (string.IsNullOrEmpty(deviceName))
            {
                logger.Error("Device not found. Please run shake-cli audio list to se a list of available devices");
                return;
            }
            logger.Debug("Selected Device: {DeviceName}", deviceName);

            if (delay != 0)
            {
                if (delay < 0)
                {
                    logger.Error("Invalid value for --delay. Must be an interger >= 0, actual value is {Delay}", delay);
                    return;
                }

                logger.Debug("Selected Delay: {Delay}", delay);
            }

            logger.Information("Audio shake starting: {DeviceName}, {Delay}, {UseTestAudio}", deviceName, delay, test);
            token.Register(() =>
            {
                logger.Information("Audio shake stopping: {DeviceName}, {Delay}, {UseTestAudio}", deviceName, delay, test);
            });
            await AudioUtils.LoopAudioWithDelay(device, delay, test, token);
        }, deviceOpt, delayOpt, testOpt);

        return audioCmd;
    }
}
