using shake.core;
using System.CommandLine;

var deviceOpt = new Option<int>("--device", description: "The audio device to keep awake. For Device list with device numbers see 'shake audio list'");
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

audioCmd.SetHandler( async (int device, int delay, bool test, CancellationToken token) =>
{
    Console.WriteLine($"Selected Device: {AudioUtils.GetAudioDeviceName(device)}");
    Console.WriteLine($"Selected Delay: {delay}");

    await AudioUtils.LoopAudioWithDelay(device, delay, test, token);
}, deviceOpt, delayOpt, testOpt);

var root = new RootCommand
{
    audioCmd
};

return root.Invoke(args);
