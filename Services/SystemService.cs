using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using ProcessManagerApp.Models;

namespace ProcessManagerApp.Services
{
    public class SystemService
    {
        public static float GetCpuUsage()
        {
            try
            {
                using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total")
                {
                    MachineName = "."
                };
                cpuCounter.NextValue();
                System.Threading.Thread.Sleep(500);
                return cpuCounter.NextValue();
            }
            catch
            {
                return 0;
            }
        }

        public static float GetMemoryUsage()
        {
            try
            {
                using var memCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use")
                {
                    MachineName = "."
                };
                return memCounter.NextValue();
            }
            catch
            {
                return 0;
            }
        }

        public static ulong GetTotalMemory()
        {
            try
            {
                return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            }
            catch
            {
                return 0;
            }
        }

        public static ulong GetAvailableMemory()
        {
            try
            {
                return new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;
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
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (var obj in searcher.Get())
                {
                    return $"{obj["Name"]} ({obj["NumberOfCores"]} cores, {obj["NumberOfLogicalProcessors"]} logical processors)";
                }
            }
            catch { }
            return "Unknown";
        }

        public static List<DiskInfo> GetDiskInfo()
        {
            var disks = new List<DiskInfo>();
            try
            {
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
            }
            catch { }
            return disks;
        }

        public static List<GpuInfo> GetGpuInfo()
        {
            var gpus = new List<GpuInfo>();
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                foreach (var obj in searcher.Get())
                {
                    try
                    {
                        gpus.Add(new GpuInfo
                        {
                            Name = obj["Name"]?.ToString() ?? "Unknown",
                            Memory = obj["AdapterRAM"] != null ? Convert.ToDouble(obj["AdapterRAM"]) / (1024.0 * 1024.0 * 1024.0) : 0,
                            DriverVersion = obj["DriverVersion"]?.ToString() ?? "Unknown"
                        });
                    }
                    catch { }
                }
            }
            catch { }
            return gpus;
        }

        public static NetworkInfo GetNetworkInfo()
        {
            var networkInfo = new NetworkInfo { Adapters = new List<string>() };
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetEnabled = true");
                foreach (var obj in searcher.Get())
                {
                    try
                    {
                        networkInfo.Adapters.Add(obj["Name"]?.ToString() ?? "Unknown");
                    }
                    catch { }
                }
            }
            catch { }
            return networkInfo;
        }

        public static List<NetworkAdapterInfo> GetNetworkAdapterDetails()
        {
            var adapters = new List<NetworkAdapterInfo>();
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetEnabled = true");
                foreach (var obj in searcher.Get())
                {
                    try
                    {
                        adapters.Add(new NetworkAdapterInfo
                        {
                            Name = obj["Name"]?.ToString() ?? "Unknown",
                            Description = obj["Description"]?.ToString() ?? "Unknown",
                            MACAddress = obj["MACAddress"]?.ToString() ?? "Unknown",
                            Speed = obj["Speed"]?.ToString() ?? "Unknown"
                        });
                    }
                    catch { }
                }
            }
            catch { }
            return adapters;
        }

        public static List<ServiceInfo> GetServices()
        {
            var services = new List<ServiceInfo>();
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service");
                foreach (var obj in searcher.Get())
                {
                    try
                    {
                        services.Add(new ServiceInfo
                        {
                            Name = obj["Name"]?.ToString() ?? "Unknown",
                            DisplayName = obj["DisplayName"]?.ToString() ?? "Unknown",
                            State = obj["State"]?.ToString() ?? "Unknown",
                            StartMode = obj["StartMode"]?.ToString() ?? "Unknown",
                            StartName = obj["StartName"]?.ToString() ?? "Unknown"
                        });
                    }
                    catch { }
                }
            }
            catch { }
            return services;
        }

        public static List<StartupInfo> GetStartupPrograms()
        {
            var startupPrograms = new List<StartupInfo>();
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_StartupCommand");
                foreach (var obj in searcher.Get())
                {
                    try
                    {
                        startupPrograms.Add(new StartupInfo
                        {
                            Name = obj["Name"]?.ToString() ?? "Unknown",
                            Command = obj["Command"]?.ToString() ?? "Unknown",
                            Location = obj["Location"]?.ToString() ?? "Unknown",
                            User = obj["User"]?.ToString() ?? "Unknown"
                        });
                    }
                    catch { }
                }
            }
            catch { }
            return startupPrograms;
        }

        public static void Shutdown()
        {
            try
            {
                Process.Start("shutdown", "/s /t 0");
            }
            catch { }
        }

        public static void Restart()
        {
            try
            {
                Process.Start("shutdown", "/r /t 0");
            }
            catch { }
        }

        public static void LogOff()
        {
            try
            {
                Process.Start("shutdown", "/l /t 0");
            }
            catch { }
        }

        public static void Sleep()
        {
            try
            {
                Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
            }
            catch { }
        }

        public static void Hibernate()
        {
            try
            {
                Process.Start("shutdown", "/h /t 0");
            }
            catch { }
        }
    }

    public class NetworkAdapterInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MACAddress { get; set; } = string.Empty;
        public string Speed { get; set; } = string.Empty;
    }

    public class ServiceInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string StartMode { get; set; } = string.Empty;
        public string StartName { get; set; } = string.Empty;
    }

    public class StartupInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
    }
}
