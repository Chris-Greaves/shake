using NAudio.Wave;

namespace Shake.Core.Utils
{
    public static class AudioUtils
    {
        public static IEnumerable<string> GetAudioDevices()
        {
            //for (int n = -1; n < WaveOut.DeviceCount; n++)
            //{
            //    var caps = WaveOut.GetCapabilities(n);
            //    yield return $"{n}: {caps.ProductName} | {caps.NameGuid}";
            //}

            foreach (var dev in DirectSoundOut.Devices)
            {
                yield return $"{dev.Guid} | {dev.Description}";
            }
        }

        public static string GetAudioDeviceName(Guid deviceId)
        {
            //try
            //{
                //var caps = WaveOut.GetCapabilities(deviceNumber);
                //return caps.ProductName;

            var device = DirectSoundOut.Devices.SingleOrDefault(d => d.Guid == deviceId);

            return device is null ? String.Empty : device.Description;

            //}
            //catch (NAudio.MmException)
            //{
            //    return string.Empty;
            //}
        }

        public static async Task LoopAudioWithDelay(Guid deviceId, int delay, bool test, CancellationToken token)
        {
            var audioFileLocation = test ? "./Sounds/test.wav" : "./Sounds/anti-sleep.wav";

            using (var audioReader = new AudioFileReader(audioFileLocation))
            using (var outputDevice = new DirectSoundOut(deviceId))
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