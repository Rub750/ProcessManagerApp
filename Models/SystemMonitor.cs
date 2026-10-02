using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessManagerApp.Models
{
    public class SystemMonitor : INotifyPropertyChanged, IDisposable
    {
        private readonly Timer _cpuTimer;
        private readonly Timer _memoryTimer;
        private readonly Timer _processTimer;
        private readonly Timer _diskTimer;
        private readonly Timer _networkTimer;

        private float _cpuUsage;
        private float _memoryUsage;
        private ulong _totalMemory;
        private ulong _availableMemory;
        private List<ProcessInfo> _processes = new();
        private List<DiskInfo> _disks = new();
        private NetworkInfo _networkInfo = new();
        private List<GpuInfo> _gpus = new();
        private string _processorInfo = string.Empty;
        private int _processorCount;

        public event PropertyChangedEventHandler? PropertyChanged;

        public float CpuUsage
        {
            get => _cpuUsage;
            set { _cpuUsage = value; OnPropertyChanged(nameof(CpuUsage)); }
        }

        public float MemoryUsage
        {
            get => _memoryUsage;
            set { _memoryUsage = value; OnPropertyChanged(nameof(MemoryUsage)); }
        }

        public ulong TotalMemory
        {
            get => _totalMemory;
            set { _totalMemory = value; OnPropertyChanged(nameof(TotalMemory)); }
        }

        public ulong AvailableMemory
        {
            get => _availableMemory;
            set { _availableMemory = value; OnPropertyChanged(nameof(AvailableMemory)); }
        }

        public List<ProcessInfo> Processes
        {
            get => _processes;
            set { _processes = value; OnPropertyChanged(nameof(Processes)); }
        }

        public List<DiskInfo> Disks
        {
            get => _disks;
            set { _disks = value; OnPropertyChanged(nameof(Disks)); }
        }

        public NetworkInfo NetworkInfo
        {
            get => _networkInfo;
            set { _networkInfo = value; OnPropertyChanged(nameof(NetworkInfo)); }
        }

        public List<GpuInfo> Gpus
        {
            get => _gpus;
            set { _gpus = value; OnPropertyChanged(nameof(Gpus)); }
        }

        public string ProcessorInfo
        {
            get => _processorInfo;
            set { _processorInfo = value; OnPropertyChanged(nameof(ProcessorInfo)); }
        }

        public int ProcessorCount
        {
            get => _processorCount;
            set { _processorCount = value; OnPropertyChanged(nameof(ProcessorCount)); }
        }

        public SystemMonitor()
        {
            _cpuTimer = new Timer(UpdateCpuUsage, null, 0, 1000);
            _memoryTimer = new Timer(UpdateMemoryUsage, null, 0, 1000);
            _processTimer = new Timer(UpdateProcesses, null, 0, 2000);
            _diskTimer = new Timer(UpdateDiskInfo, null, 0, 5000);
            _networkTimer = new Timer(UpdateNetworkInfo, null, 0, 10000);

            InitializeStaticInfo();
        }

        private void InitializeStaticInfo()
        {
            TotalMemory = SystemInfo.GetTotalMemory();
            AvailableMemory = SystemInfo.GetAvailableMemory();
            ProcessorInfo = SystemInfo.GetProcessorInfo();
            ProcessorCount = SystemInfo.GetProcessorCount();
            Disks = SystemInfo.GetDiskInfo();
            Gpus = SystemInfo.GetGpuInfo();
        }

        private void UpdateCpuUsage(object? state)
        {
            CpuUsage = SystemInfo.GetCpuUsage();
        }

        private void UpdateMemoryUsage(object? state)
        {
            AvailableMemory = SystemInfo.GetAvailableMemory();
            TotalMemory = SystemInfo.GetTotalMemory();
            MemoryUsage = SystemInfo.GetMemoryUsage();
        }

        private void UpdateProcesses(object? state)
        {
            Processes = SystemInfo.GetProcesses();
        }

        private void UpdateDiskInfo(object? state)
        {
            Disks = SystemInfo.GetDiskInfo();
        }

        private void UpdateNetworkInfo(object? state)
        {
            NetworkInfo = SystemInfo.GetNetworkInfo();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _cpuTimer?.Dispose();
            _memoryTimer?.Dispose();
            _processTimer?.Dispose();
            _diskTimer?.Dispose();
            _networkTimer?.Dispose();
        }
    }
}
