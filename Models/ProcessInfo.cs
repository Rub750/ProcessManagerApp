using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ProcessManagerApp.Models
{
    public enum ProcessPriority
    {
        Idle,
        BelowNormal,
        Normal,
        AboveNormal,
        High,
        Realtime
    }

    public enum ProcessCategory
    {
        System,
        Application,
        Background,
        Service,
        Unknown
    }

    public class ProcessInfo
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string WindowTitle { get; set; }
        public string ExecutablePath { get; set; }
        public ProcessPriority Priority { get; set; }
        public ProcessCategory Category { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Uptime => DateTime.Now - StartTime;
        public long MemoryUsage { get; set; }
        public long CpuUsage { get; set; }
        public int ThreadCount { get; set; }
        public int HandleCount { get; set; }
        public bool IsCritical { get; set; }
        public Icon ProcessIcon { get; set; }

        // Convertir ProcessPriority en string
        public string PriorityString => Priority.ToString();

        // Convertir ProcessCategory en string
        public string CategoryString => Category.ToString();

        // Taille de la mémoire en Mo
        public string MemoryUsageMB => $"{(MemoryUsage / (1024.0 * 1024.0)):F2} Mo";

        // Temps d'activité formaté
        public string UptimeString
        {
            get
            {
                if (Uptime.TotalDays >= 1)
                    return $"{Uptime.Days} jours, {Uptime.Hours} heures, {Uptime.Minutes} minutes";
                else if (Uptime.TotalHours >= 1)
                    return $"{Uptime.Hours} heures, {Uptime.Minutes} minutes, {Uptime.Seconds} secondes";
                else if (Uptime.TotalMinutes >= 1)
                    return $"{Uptime.Minutes} minutes, {Uptime.Seconds} secondes";
                else
                    return $"{Uptime.Seconds} secondes";
            }
        }

        // Usage CPU en pourcentage
        public string CpuUsagePercent => $"{CpuUsage:F1}%";

        // Créer un ProcessInfo à partir d'un Process
        public static ProcessInfo FromProcess(Process process)
        {
            try
            {
                var processInfo = new ProcessInfo
                {
                    ProcessId = process.Id,
                    ProcessName = process.ProcessName,
                    StartTime = process.StartTime,
                    MemoryUsage = process.WorkingSet64,
                    ThreadCount = process.Threads.Count,
                    HandleCount = process.HandleCount,
                    ExecutablePath = GetExecutablePath(process),
                    WindowTitle = GetWindowTitle(process),
                    Priority = GetPriority(process),
                    Category = GetCategory(process),
                    IsCritical = IsCriticalProcess(process.ProcessName),
                    ProcessIcon = GetProcessIcon(process)
                };

                // Calculer l'usage CPU (simplifié)
                var startTime = DateTime.Now;
                var startCpuUsage = process.TotalProcessorTime;
                System.Threading.Thread.Sleep(100);
                var endTime = DateTime.Now;
                var endCpuUsage = process.TotalProcessorTime;
                
                var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                var totalSpanMs = (endTime - startTime).TotalMilliseconds;
                var totalCpuPercentage = (cpuUsedMs / (Environment.ProcessorCount * totalSpanMs)) * 100;
                
                processInfo.CpuUsage = (long)totalCpuPercentage;

                return processInfo;
            }
            catch
            {
                return null;
            }
        }

        private static string GetExecutablePath(Process process)
        {
            try
            {
                return process.MainModule?.FileName;
            }
            catch
            {
                return "Accès refusé";
            }
        }

        private static string GetWindowTitle(Process process)
        {
            try
            {
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    var length = GetWindowTextLength(process.MainWindowHandle);
                    if (length > 0)
                    {
                        var title = new System.Text.StringBuilder(length + 1);
                        GetWindowText(process.MainWindowHandle, title, title.Capacity);
                        return title.ToString();
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        private static ProcessPriority GetPriority(Process process)
        {
            try
            {
                return (ProcessPriority)process.PriorityClass;
            }
            catch
            {
                return ProcessPriority.Normal;
            }
        }

        private static ProcessCategory GetCategory(Process process)
        {
            var processName = process.ProcessName.ToLower();
            
            // Processus système
            if (processName.Contains("system") || processName.Contains("smss") || 
                processName.Contains("csrss") || processName.Contains("wininit") ||
                processName.Contains("services") || processName.Contains("lsass"))
                return ProcessCategory.System;
            
            // Services
            if (processName.Contains("svchost") || processName.Contains("spoolsv") ||
                processName.Contains("taskhostw") || processName.Contains("dllhost"))
                return ProcessCategory.Service;
            
            // Applications connues
            if (processName.Contains("explorer") || processName.Contains("chrome") ||
                processName.Contains("firefox") || processName.Contains("edge") ||
                processName.Contains("word") || processName.Contains("excel") ||
                processName.Contains("notepad") || processName.Contains("calc"))
                return ProcessCategory.Application;
            
            // Processus en arrière-plan
            if (processName.Contains("runtimebroker") || processName.Contains("sihost") ||
                processName.Contains("ctfmon") || processName.Contains("searchindexer"))
                return ProcessCategory.Background;
            
            return ProcessCategory.Unknown;
        }

        private static bool IsCriticalProcess(string processName)
        {
            var criticalProcesses = new[]
            {
                "System", "smss", "csrss", "wininit", "services", "lsass", 
                "svchost", "spoolsv", "taskhostw", "explorer", "dwm"
            };
            
            return Array.Exists(criticalProcesses, p => 
                processName.Equals(p, StringComparison.OrdinalIgnoreCase));
        }

        private static Icon GetProcessIcon(Process process)
        {
            try
            {
                if (process.MainModule != null)
                {
                    return Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        // Importations pour obtenir le titre de la fenêtre
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
    }
}
