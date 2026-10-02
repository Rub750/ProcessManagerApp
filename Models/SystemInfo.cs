using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProcessManagerApp.Models
{
    public class SystemInfo
    {
        public static float GetCpuUsage()
        {
            using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total")
            {
                MachineName = "."
            };
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(500);
            return cpuCounter.NextValue();
        }

        public static ulong GetTotalMemory()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

        public static ulong GetAvailableMemory()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;
        }

        public static float GetMemoryUsage()
        {
            var total = GetTotalMemory();
            var available = GetAvailableMemory();
            return total > 0 ? ((total - available) / (float)total) * 100 : 0;
        }

        public static List<ProcessInfo> GetProcesses()
        {
            var processes = Process.GetProcesses()
                .Select(p => new ProcessInfo
                {
                    Id = p.Id,
                    Name = p.ProcessName,
                    CpuUsage = GetProcessCpuUsage(p),
                    MemoryUsage = p.WorkingSet64 / (1024.0 * 1024.0),
                    StartTime = p.StartTime,
                    Responding = p.Responding
                })
                .OrderByDescending(p => p.CpuUsage)
                .ThenByDescending(p => p.MemoryUsage)
                .ToList();
            return processes;
        }

        public static List<ProcessInfo> GetTopProcesses(int count = 10)
        {
            return GetProcesses().Take(count).ToList();
        }

        private static float GetProcessCpuUsage(Process process)
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

        public static int GetProcessorCount()
        {
            return Environment.ProcessorCount;
        }

        public static string GetProcessorInfo()
        {
            try
            {
                using var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (var obj in searcher.Get())
                {
                    return $"{obj["Name"]} ({obj["NumberOfCores"]} cores, {obj["NumberOfLogicalProcessors"]} logical processors)";
                }
            }
            catch
            {
                return "Unknown";
            }
            return "Unknown";
        }

        public static List<DiskInfo> GetDiskInfo()
        {
            var disks = new List<DiskInfo>();
            foreach (var drive in System.IO.DriveInfo.GetDrives())
            {
                try
                {
                    if (drive.IsReady)
                    {
                        disks.Add(new DiskInfo
                        {
                            Name = drive.Name,
                            Label = drive.VolumeLabel,
                            TotalSpace = drive.TotalSize / (1024.0 * 1024.0 * 1024.0),
                            FreeSpace = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0),
                            UsedSpace = (drive.TotalSize - drive.AvailableFreeSpace) / (1024.0 * 1024.0 * 1024.0),
                            UsagePercent = drive.TotalSize > 0 ? ((drive.TotalSize - drive.AvailableFreeSpace) / (double)drive.TotalSize) * 100 : 0
                        });
                    }
                }
                catch { }
            }
            return disks;
        }

        public static NetworkInfo GetNetworkInfo()
        {
            try
            {
                using var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetEnabled = true");
                var adapters = new List<string>();
                foreach (var obj in searcher.Get())
                {
                    adapters.Add(obj["Name"].ToString());
                }
                return new NetworkInfo { Adapters = adapters };
            }
            catch
            {
                return new NetworkInfo { Adapters = new List<string>() };
            }
        }

        public static List<GpuInfo> GetGpuInfo()
        {
            var gpus = new List<GpuInfo>();
            try
            {
                using var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                foreach (var obj in searcher.Get())
                {
                    gpus.Add(new GpuInfo
                    {
                        Name = obj["Name"].ToString(),
                        Memory = obj["AdapterRAM"] != null ? Convert.ToDouble(obj["AdapterRAM"]) / (1024.0 * 1024.0 * 1024.0) : 0,
                        DriverVersion = obj["DriverVersion"]?.ToString() ?? "Unknown"
                    });
                }
            }
            catch { }
            return gpus;
        }
    }

    public class ProcessInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public DateTime StartTime { get; set; }
        public bool Responding { get; set; }
    }

    public class DiskInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public double TotalSpace { get; set; }
        public double FreeSpace { get; set; }
        public double UsedSpace { get; set; }
        public double UsagePercent { get; set; }
    }

    public class NetworkInfo
    {
        public List<string> Adapters { get; set; } = new();
    }

    public class GpuInfo
    {
        public string Name { get; set; } = string.Empty;
        public double Memory { get; set; }
        public string DriverVersion { get; set; } = string.Empty;
    }
}
