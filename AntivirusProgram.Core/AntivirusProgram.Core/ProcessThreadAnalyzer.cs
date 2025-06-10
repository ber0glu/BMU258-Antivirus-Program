using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AntivirusProgram.Core
{
    public static class ProcessThreadAnalyzer
    {
        [DllImport("kernel32.dll")]
        private static extern uint GetProcessIdOfThread(IntPtr thread);

        public static bool HasRemoteThread(int targetPid)
        {
            foreach (ProcessThread thread in Process.GetCurrentProcess().Threads)
            {
                if (thread.Id == targetPid)
                    continue;

                try
                {
                    using Process owner = Process.GetProcessById(thread.Id);
                    if (owner.Id == targetPid)
                        return true;
                }
                catch
                {
                    // ignore
                }
            }

            return false;
        }
    }
}
