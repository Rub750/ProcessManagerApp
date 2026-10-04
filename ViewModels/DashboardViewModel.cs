using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using ProcessManagerApp.Models;

namespace ProcessManagerApp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly SystemMonitor _systemMonitor;
        private float _cpuUsageHistoryMax = 100;
        private float _memoryUsageHistoryMax = 100;
        private List<float> _cpuUsageHistory = new();
        private List<float> _memoryUsageHistory = new();
        private string _systemSummary = string.Empty;
        private DateTime _lastUpdate = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SystemMonitor SystemMonitor => _systemMonitor;

        public float CpuUsageHistoryMax
        {
            get => _cpuUsageHistoryMax;
            set
            {
                _cpuUsageHistoryMax = value;
                OnPropertyChanged();
            }
        }

        public float MemoryUsageHistoryMax
        {
            get => _memoryUsageHistoryMax;
            set
            {
                _memoryUsageHistoryMax = value;
                OnPropertyChanged();
            }
        }

        public List<float> CpuUsageHistory
        {
            get => _cpuUsageHistory;
            set
            {
                _cpuUsageHistory = value;
                OnPropertyChanged();
            }
        }

        public List<float> MemoryUsageHistory
        {
            get => _memoryUsageHistory;
            set
            {
                _memoryUsageHistory = value;
                OnPropertyChanged();
            }
        }

        public string SystemSummary
        {
            get => _systemSummary;
            set
            {
                _systemSummary = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set
            {
                _lastUpdate = value;
                OnPropertyChanged();
            }
        }

        public DashboardViewModel(SystemMonitor systemMonitor)
        {
            _systemMonitor = systemMonitor;
            _systemMonitor.PropertyChanged += SystemMonitor_PropertyChanged;

            InitializeHistory();
            UpdateSummary();

            Task.Run(() => UpdateHistoryAsync());
        }

        private void SystemMonitor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SystemMonitor.CpuUsage) ||
                e.PropertyName == nameof(SystemMonitor.MemoryUsage))
            {
                UpdateHistory();
                UpdateSummary();
            }
        }

        private void InitializeHistory()
        {
            CpuUsageHistory = Enumerable.Range(0, 60).Select(_ => 0f).ToList();
            MemoryUsageHistory = Enumerable.Range(0, 60).Select(_ => 0f).ToList();
        }

        private async Task UpdateHistoryAsync()
        {
            while (true)
            {
                await Task.Delay(1000);
                UpdateHistory();
                UpdateSummary();
            }
        }

        private void UpdateHistory()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    CpuUsageHistory.Add(_systemMonitor.CpuUsage);
                    if (CpuUsageHistory.Count > 60)
                    {
                        CpuUsageHistory.RemoveAt(0);
                    }
                    CpuUsageHistoryMax = Math.Max(CpuUsageHistoryMax, _systemMonitor.CpuUsage);

                    MemoryUsageHistory.Add(_systemMonitor.MemoryUsage);
                    if (MemoryUsageHistory.Count > 60)
                    {
                        MemoryUsageHistory.RemoveAt(0);
                    }
                    MemoryUsageHistoryMax = Math.Max(MemoryUsageHistoryMax, _systemMonitor.MemoryUsage);

                    LastUpdate = DateTime.Now;
                }
                catch { }
            });
        }

        private void UpdateSummary()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var cpuUsage = _systemMonitor.CpuUsage;
                    var memoryUsage = _systemMonitor.MemoryUsage;
                    var totalMemory = _systemMonitor.TotalMemory / (1024.0 * 1024.0 * 1024.0);
                    var availableMemory = _systemMonitor.AvailableMemory / (1024.0 * 1024.0 * 1024.0);
                    var usedMemory = totalMemory - availableMemory;

                    var diskInfo = _systemMonitor.Disks.FirstOrDefault();
                    var diskUsage = diskInfo != null ? diskInfo.UsagePercent : 0;

                    SystemSummary =
                        $"CPU: {cpuUsage:F1}% | " +
                        $"RAM: {memoryUsage:F1}% ({usedMemory:F2} GB / {totalMemory:F2} GB) | " +
                        $"Disque: {diskUsage:F1}% | " +
                        $"Processeurs: {_systemMonitor.ProcessorCount} | " +
                        $"Mise à jour: {LastUpdate:HH:mm:ss}";
                }
                catch { }
            });
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
