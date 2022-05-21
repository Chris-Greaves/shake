using NAudio.Wave;

namespace Shake.Core
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

        public static string GetAudioDeviceName(int deviceNumber)
        {
            try
            {
                var caps = WaveOut.GetCapabilities(deviceNumber);
                return caps.ProductName;
            }
            catch (NAudio.MmException)
            {
                return String.Empty;
            }
        }

        public static async Task LoopAudioWithDelay(int deviceNumber, int delay, bool test, CancellationToken token)
        {
            var audioFileLocation = test ? "./Sounds/test.wav" : "./Sounds/anti-sleep.wav";

            using (var audioReader = new AudioFileReader(audioFileLocation))
            using (var outputDevice = new WaveOutEvent() { DeviceNumber = deviceNumber })
            {
                outputDevice.Init(audioReader);
                while (!token.IsCancellationRequested)
                {
                    audioReader.Position = 0;
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        // Sleep until the audio has stopped playing
                        await Task.Delay(250, token);
                    }

                    outputDevice.Stop();
                    // Sleep for the delay
                    await Task.Delay(delay * 1000, token);
                }
            }
        }
    }
}