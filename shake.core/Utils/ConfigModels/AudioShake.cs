namespace Shake.Core.Utils.ConfigModels;

public class AudioShake
{
    public string Id { get; set; }
    public Guid AudioDevice { get; set; }
    public long Delay { get; set; }
    public bool UseTestAudio { get; set; }
}
