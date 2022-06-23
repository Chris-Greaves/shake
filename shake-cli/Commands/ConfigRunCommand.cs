using Serilog;
using Shake.Core.Utils;
using Shake.Core.Utils.ConfigModels;
using System.CommandLine;

namespace Shake.CLI.Commands;
internal class ConfigRunCommand
{
    public static Command GetCommand(ILogger logger)
    {
        var watchOpt = new Option<bool>(new string[] { "--watch", "-w" }, "Set flag to watch for changes in the config and re-run on change");
        var runCmd = new Command("run", "Runs all shakes defined in the current global config")
        {
            watchOpt
        };

        runCmd.SetHandler(async (bool watch, CancellationToken token) =>
        {
            while (!token.IsCancellationRequested)
            {
                var config = ConfigUtils.GetConfig();

                if (watch)
                {
                    using (var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token))
                    using (var watcher = ConfigUtils.SetupConfigWatch((object sender, FileSystemEventArgs e) =>
                    {
                        logger.Warning("Config changed, refreshing shakes.");
                        tokenSource.Cancel();
                    }))
                    {
                        try
                        {
                            logger.Information("Starting up shakes from config.");
                            await RunConfig(logger, config, tokenSource.Token);
                        }
                        catch (OperationCanceledException) { }
                    }
                }
                else
                    await RunConfig(logger, config, token);
            }
        }, watchOpt);

        return runCmd;
    }

    static async Task RunConfig(ILogger logger, ShakeOptions config, CancellationToken token)
    {
        var shakes = new List<Task>();

        foreach (var aShake in config.AudioShakes)
        {
            logger.Information("Starting an audio shake: {DeviceName}, {Delay}, {UseTestAudio}", aShake.AudioDevice, aShake.Delay, aShake.UseTestAudio);
            shakes.Add(AudioUtils.LoopAudioWithDelay(aShake.AudioDevice, aShake.Delay, aShake.UseTestAudio, token));
        }

        if (config.VideoShake is not null)
        {
            logger.Information("Starting the video shake: {IncludeDisplay}, {Timer}", config.VideoShake.IncludeDisplay, config.VideoShake.Timer);
            shakes.Add(VideoUtils.SetupAwakeLoopAsync(config.VideoShake.IncludeDisplay, config.VideoShake.Timer, token));
        }

        await Task.WhenAll(shakes.AsEnumerable());
        logger.Information("All shakes finished");
    }
}
