using Shake.Core.Utils;
using Shake.Core.Utils.ConfigModels;
using System;
using System.CommandLine;

var audioCmd = CreateAudioCommand();
var screenCmd = CreateScreenCommand();
var configCmd = CreateConfigCommand();

var root = new RootCommand
{
    audioCmd,
    screenCmd,
    configCmd
};

return root.Invoke(args);

Command CreateAudioCommand()
{
    var deviceOpt = new Option<Guid>("--device", description: "The audio device to keep awake. For a list of devices with ids see 'shake audio list'");
    deviceOpt.AddAlias("-d");
    var delayOpt = new Option<int>("--delay", getDefaultValue: () => 5, description: "The delay between audio cues in seconds");
    var testOpt = new Option<bool>("--test", getDefaultValue: () => false, description: "Switch to use the audiable file to test it works");

    var listAudioCmd = new Command("list", description: "List audio devices to keep awake");

    listAudioCmd.SetHandler(() =>
    {
        foreach (var device in AudioUtils.GetAudioDevices())
        {
            Console.WriteLine(device);
        }
    });

    var audioCmd = new Command("audio", description: "Keep and audio device Awake by playing inaudible sounds on repeat")
    {
        deviceOpt,
        delayOpt,
        testOpt,
        listAudioCmd
    };

    audioCmd.SetHandler(async (Guid device, int delay, bool test, CancellationToken token) =>
    {
        var deviceName = AudioUtils.GetAudioDeviceName(device);
        if (string.IsNullOrEmpty(deviceName))
        {
            Console.WriteLine("Device not found. Please run shake-cli audio list to se a list of available devices");
            return;
        }
        Console.WriteLine($"Selected Device: {deviceName}");

        if (delay != 0)
        {
            if (delay < 0)
            {
                Console.WriteLine($"Invalid value for --delay. Must be an interger >= 0, actual value is {delay}");
                return;
            }

            Console.WriteLine($"Selected Delay: {delay}");
        }

        await AudioUtils.LoopAudioWithDelay(device, delay, test, token);
    }, deviceOpt, delayOpt, testOpt);

    return audioCmd;
}

Command CreateScreenCommand()
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
            Console.WriteLine("Param: User selected to keep display awake as well");
        }
        if (timer != 0)
        {
            if (timer < 0)
            {
                Console.WriteLine($"Invalid value for --timer. Must be >= 0, actual value was {timer}");
                return;
            }
            Console.WriteLine($"Param: Timer set for {timer} seconds");
        }

        await VideoUtils.SetupAwakeLoopAsync(includeDisplay, timer, token);
    }, displayOpt, timerOpt);

    return cmd;
}

Command CreateConfigCommand()
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

    var displayOpt = new Option<bool>("--display", getDefaultValue: () => false, description: "Switch to ensure the display also doesn't go to sleep");
    displayOpt.AddAlias("-d");
    var timerOpt = new Option<int>("--timer", getDefaultValue: () => 0, description: "Set a timer in seconds for the Shake to keep the PC awake");
    timerOpt.AddAlias("-t");

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

    var clearScreenCmd = new Command("clear-screen", description: "Clear any screen settings from config");

    clearScreenCmd.SetHandler(() =>
    {
        var config = ConfigUtils.GetConfig();
        config.VideoShake = null;
        config.Save();
    });

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
                    Console.WriteLine("Config changed, refreshing");
                    tokenSource.Cancel();
                }))
                {
                    try
                    {
                        await RunConfig(config, tokenSource.Token);
                    }
                    catch (OperationCanceledException) { }
                }
            }
            else
                await RunConfig(config, token);
        }
    }, watchOpt);

    var cmd = new Command("config", description: "Update the global config for shake.")
    {
        addAudioCmd,
        removeAudioCmd,
        setScreenCmd,
        clearScreenCmd,
        runCmd
    };

    cmd.SetHandler(() =>
    {
        Console.Write(ConfigUtils.GetConfigAsString());
    });

    return cmd;
}

static async Task RunConfig(ShakeOptions config, CancellationToken token)
{
    var shakes = new List<Task>();

    foreach (var aShake in config.AudioShakes)
    {
        shakes.Add(AudioUtils.LoopAudioWithDelay(aShake.AudioDevice, aShake.Delay, aShake.UseTestAudio, token));
    }

    if (config.VideoShake is not null)
    {
        shakes.Add(VideoUtils.SetupAwakeLoopAsync(config.VideoShake.IncludeDisplay, config.VideoShake.Timer, token));
    }

    await Task.WhenAll(shakes.AsEnumerable());
}