using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace ProcessManagerApp.Models
{
    public class PerformanceOptimizer
    {
        [DllImport("kernel32.dll")]
        private static extern bool SetPriorityClass(IntPtr hProcess, uint dwPriorityClass);

        [DllImport("kernel32.dll")]
        private static extern bool SetThreadPriority(IntPtr hThread, int nPriority);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        private const uint REALTIME_PRIORITY_CLASS = 0x00000100;
        private const uint HIGH_PRIORITY_CLASS = 0x00000080;
        private const uint ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000;
        private const uint NORMAL_PRIORITY_CLASS = 0x00000020;
        private const uint BELOW_NORMAL_PRIORITY_CLASS = 0x00010000;
        private const uint IDLE_PRIORITY_CLASS = 0x00000040;

        public static void OptimizeCpuForGaming()
        {
            try
            {
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    try
                    {
                        if (IsBackgroundProcess(process.ProcessName))
                        {
                            var handle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
                            if (handle != IntPtr.Zero)
                            {
                                SetPriorityClass(handle, BELOW_NORMAL_PRIORITY_CLASS);
                                CloseHandle(handle);
                            }
                        }
                        else if (IsGameProcess(process.ProcessName))
                        {
                            var handle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
                            if (handle != IntPtr.Zero)
                            {
                                SetPriorityClass(handle, HIGH_PRIORITY_CLASS);
                                CloseHandle(handle);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static void OptimizeCpuForProductivity()
        {
            try
            {
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    try
                    {
                        var handle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
                        if (handle != IntPtr.Zero)
                        {
                            SetPriorityClass(handle, NORMAL_PRIORITY_CLASS);
                            CloseHandle(handle);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static void OptimizeCpuForBatterySaving()
        {
            try
            {
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    try
                    {
                        if (!IsCriticalProcess(process.ProcessName))
                        {
                            var handle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
                            if (handle != IntPtr.Zero)
                            {
                                SetPriorityClass(handle, IDLE_PRIORITY_CLASS);
                                CloseHandle(handle);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static void KillProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                process.Kill();
                process.WaitForExit();
            }
            catch { }
        }

        public static void KillProcessByName(string processName)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);
                foreach (var process in processes)
                {
                    process.Kill();
                    process.WaitForExit();
                }
            }
            catch { }
        }

        public static void EndTask(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                if (!process.CloseMainWindow())
                {
                    process.Kill();
                }
            }
            catch { }
        }

        public static void SuspendProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                foreach (ProcessThread thread in process.Threads)
                {
                    var threadHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
                    if (threadHandle != IntPtr.Zero)
                    {
                        SetThreadPriority(threadHandle, -2); // THREAD_PRIORITY_IDLE
                        CloseHandle(threadHandle);
                    }
                }
            }
            catch { }
        }

        public static void ResumeProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                foreach (ProcessThread thread in process.Threads)
                {
                    var threadHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
                    if (threadHandle != IntPtr.Zero)
                    {
                        SetThreadPriority(threadHandle, 0); // THREAD_PRIORITY_NORMAL
                        CloseHandle(threadHandle);
                    }
                }
            }
            catch { }
        }

        public static void CleanMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
            catch { }
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);

        private static bool IsBackgroundProcess(string processName)
        {
            var backgroundProcesses = new List<string>
            {
                "chrome", "firefox", "msedge", "opera", "safari",
                "spotify", "discord", "telegram", "skype", "slack",
                "steam", "epicgameslauncher", "origin", "ubisoftgamelauncher",
                "nvidia", "rtss", "msiafterburner", "hwmonitor", "cpuz"
            };
            return backgroundProcesses.Any(p => processName.Equals(p, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsGameProcess(string processName)
        {
            var gameProcesses = new List<string>
            {
                "game", "gta", "assassin", "call", "battle", "apex", "fortnite",
                "minecraft", "valorant", "lol", "dota", "cs2", "csgo"
            };
            return gameProcesses.Any(p => processName.Equals(p, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsCriticalProcess(string processName)
        {
            var criticalProcesses = new List<string>
            {
                "svchost", "lsass", "csrss", "wininit", "services", "winlogon",
                "explorer", "dwm", "system", "registry", "smss"
            };
            return criticalProcesses.Any(p => processName.Equals(p, StringComparison.OrdinalIgnoreCase));
        }
    }
}
