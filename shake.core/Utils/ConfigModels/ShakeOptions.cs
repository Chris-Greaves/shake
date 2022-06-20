namespace Shake.Core.Utils.ConfigModels;
public class ShakeOptions
{
    public List<AudioShake> AudioShakes { get; set; } = new List<AudioShake>();

    public VideoShake? VideoShake { get; set; }
}
