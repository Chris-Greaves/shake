using shake.core;
using System.CommandLine;

var deviceOpt = new Option<string>("--device", getDefaultValue: () => "default", description: "The audio device to keep awake");
var delayOpt = new Option<int>("--delay", getDefaultValue: () => 5, description: "The delay between audio cues");

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
    listAudioCmd
};

audioCmd.SetHandler((string device, int delay) =>
{
    Console.WriteLine("Set Audio Device to keep awake called");
    Console.WriteLine($"Selected Device: {device}");
    Console.WriteLine($"Selected Delay: {delay}");
}, deviceOpt, delayOpt);

var root = new RootCommand
{
    audioCmd
};

return root.Invoke(args);
