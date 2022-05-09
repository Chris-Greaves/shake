using NAudio.Wave;

namespace shake.core
{
    public static class AudioUtils
    {
        public static IEnumerable<string> GetAudioDevices()
        {
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                yield return $"{n}: {caps.ProductName}";
            }
        }
    }
}