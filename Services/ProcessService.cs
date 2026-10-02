using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using ProcessManagerApp.Models;

namespace ProcessManagerApp.Services
{
    public class ProcessService
    {
        public List<ProcessInfo> GetProcesses()
        {
            try
            {
                return Process.GetProcesses()
                    .Select(p => new ProcessInfo
                    {
                        Id = p.Id,
                        Name = p.ProcessName,
                        CpuUsage = GetProcessCpuUsage(p),
                        MemoryUsage = p.WorkingSet64 / (1024.0 * 1024.0),
                        StartTime = p.StartTime,
                        Responding = p.Responding,
                        Priority = p.PriorityClass.ToString()
                    })
                    .OrderByDescending(p => p.CpuUsage)
                    .ThenByDescending(p => p.MemoryUsage)
                    .ToList();
            }
            catch
            {
                return new List<ProcessInfo>();
            }
        }

        public List<ProcessInfo> GetProcessesByName(string name)
        {
            try
            {
                return Process.GetProcessesByName(name)
                    .Select(p => new ProcessInfo
                    {
                        Id = p.Id,
                        Name = p.ProcessName,
                        CpuUsage = GetProcessCpuUsage(p),
                        MemoryUsage = p.WorkingSet64 / (1024.0 * 1024.0),
                        StartTime = p.StartTime,
                        Responding = p.Responding
                    })
                    .ToList();
            }
            catch
            {
                return new List<ProcessInfo>();
            }
        }

        public bool KillProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                process.Kill();
                process.WaitForExit();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EndTask(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                if (!process.CloseMainWindow())
                {
                    process.Kill();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SuspendProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                foreach (ProcessThread thread in process.Threads)
                {
                    thread.Suspend();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ResumeProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                foreach (ProcessThread thread in process.Threads)
                {
                    thread.Resume();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetPriority(int processId, ProcessPriorityClass priority)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                process.PriorityClass = priority;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsProcessRunning(string processName)
        {
            try
            {
                return Process.GetProcessesByName(processName).Any();
            }
            catch
            {
                return false;
            }
        }

        public ProcessInfo? GetProcessInfo(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                return new ProcessInfo
                {
                    Id = process.Id,
                    Name = process.ProcessName,
                    CpuUsage = GetProcessCpuUsage(process),
                    MemoryUsage = process.WorkingSet64 / (1024.0 * 1024.0),
                    StartTime = process.StartTime,
                    Responding = process.Responding
                };
            }
            catch
            {
                return null;
            }
        }

        private float GetProcessCpuUsage(Process process)
        {
            try
            {
                using var cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName)
                {
                    MachineName = ".",
                    InstanceName = process.ProcessName
                };
                cpuCounter.NextValue();
                System.Threading.Thread.Sleep(100);
                return cpuCounter.NextValue() / Environment.ProcessorCount;
            }
            catch
            {
                return 0;
            }
        }

        public List<ProcessInfo> GetTopCpuProcesses(int count = 10)
        {
            return GetProcesses().OrderByDescending(p => p.CpuUsage).Take(count).ToList();
        }

        public List<ProcessInfo> GetTopMemoryProcesses(int count = 10)
        {
            return GetProcesses().OrderByDescending(p => p.MemoryUsage).Take(count).ToList();
        }

        public List<ProcessInfo> GetRecentProcesses(int count = 10)
        {
            return GetProcesses().OrderByDescending(p => p.StartTime).Take(count).ToList();
        }

        public List<ProcessInfo> GetNonRespondingProcesses()
        {
            return GetProcesses().Where(p => !p.Responding).ToList();
        }

        public void KillAllProcessesByName(string processName)
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

        public string GetProcessFilePath(int processId)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(
                    $"SELECT ExecutablePath FROM Win32_Process WHERE ProcessId = {processId}");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["ExecutablePath"]?.ToString() ?? string.Empty;
                }
            }
            catch { }
            return string.Empty;
        }
    }
}
