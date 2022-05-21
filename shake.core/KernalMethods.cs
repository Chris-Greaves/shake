using Shake.Core.KernalModels;
using System.Runtime.InteropServices;

namespace Shake.Core
{
    internal class KernalMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);
    }
}
