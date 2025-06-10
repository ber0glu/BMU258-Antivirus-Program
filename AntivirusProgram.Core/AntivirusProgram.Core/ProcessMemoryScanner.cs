using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AntivirusProgram.Core
{
    public static class ProcessMemoryScanner
    {
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;
        private const uint PROCESS_VM_READ = 0x0010;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint access, bool inheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress,
            out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        public static bool HasRWXMemoryRegion(int processId)
        {
            IntPtr hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, processId);
            if (hProcess == IntPtr.Zero)
                return false;

            try
            {
                IntPtr address = IntPtr.Zero;
                MEMORY_BASIC_INFORMATION mbi;
                while (VirtualQueryEx(hProcess, address, out mbi, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))))
                {
                    if ((mbi.Protect & PAGE_EXECUTE_READWRITE) == PAGE_EXECUTE_READWRITE)
                        return true;

                    address = new IntPtr(address.ToInt64() + (long)mbi.RegionSize);
                }
            }
            catch
            {
                // ignore or log
            }
            finally
            {
                CloseHandle(hProcess);
            }

            return false;
        }
    }
}
