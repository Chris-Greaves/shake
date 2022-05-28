using Shake.Core.Utils;
using System.CommandLine;

var audioCmd = CreateAudioCommand();
var screenCmd = CreateScreenCommand();

var root = new RootCommand
{
    audioCmd,
    screenCmd
};

return root.Invoke(args);

Command CreateAudioCommand()
{
    var deviceOpt = new Option<int>("--device", description: "The audio device to keep awake. For Device list with device numbers see 'shake audio list'");
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

    audioCmd.SetHandler(async (int device, int delay, bool test, CancellationToken token) =>
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
};

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