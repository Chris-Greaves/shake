using Shake.Core.KernalModels;
using Timer = System.Timers.Timer;

namespace Shake.Core
{
    public static class VideoUtils
    {
        /// <summary>
        /// Method for Running a Loop to keep the PC awake
        /// </summary>
        /// <param name="includeDisplay">Set to true if you want the screen to stay awake as well.</param>
        /// <param name="timer">Amount of time you want the loop to last, in seconds. If 0, it is infinite</param>
        /// <param name="token">Cancellation Token</param>
        /// <returns></returns>
        public static async Task<bool> SetupAwakeLoopAsync(bool includeDisplay, int timer, CancellationToken token)
        {
            return await Task.Run(() => SetupAwakeLoop(includeDisplay, timer, token), token);
        }

        /// <summary>
        /// Method for Running a Loop to keep the PC awake
        /// </summary>
        /// <param name="includeDisplay">Set to true if you want the screen to stay awake as well.</param>
        /// <param name="timer">Amount of time you want the loop to last, in seconds. If 0, it is infinite</param>
        /// <param name="token">Cancellation Token</param>
        /// <returns></returns>
        public static bool SetupAwakeLoop(bool includeDisplay, int timer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return true; // Return true as although it didn't do anything, it also didn't go wrong.

            // Create a token for the loop, with delay if timer is enabled.
            var loopTokenSource = timer != 0 ? new CancellationTokenSource(timer * 1000) : new CancellationTokenSource();

            bool success;
            if (includeDisplay)
            {
                success = SetAwakeState(ExecutionState.ES_SYSTEM_REQUIRED | ExecutionState.ES_DISPLAY_REQUIRED | ExecutionState.ES_CONTINUOUS);
            }
            else
            {
                success = SetAwakeState(ExecutionState.ES_SYSTEM_REQUIRED | ExecutionState.ES_CONTINUOUS);
            }

            try
            {
                if (success)
                {
                    WaitHandle.WaitAny(new[] { token.WaitHandle, loopTokenSource.Token.WaitHandle });

                    return success;
                }
            }
            catch (OperationCanceledException ex)
            {
                return success;
            }

            return false;
        }

        private static bool SetAwakeState(ExecutionState executionState)
        {
            try
            {
                var stateResult = KernalMethods.SetThreadExecutionState(executionState);
                return stateResult != 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
