using Shake.Core.KernalModels;

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

            if (timer != 0)
            {
                throw new NotImplementedException("Timer functionality not implemented yet.");
            }

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
                    WaitHandle.WaitAny(new[] { token.WaitHandle });

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
