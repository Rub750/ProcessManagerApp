using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ProcessManagerApp.Models;

namespace ProcessManagerApp.ViewModels
{
    public enum OptimizationMode
    {
        None,
        Gaming,
        Productivity,
        BatterySaving
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SystemMonitor _systemMonitor;
        private string _searchText = string.Empty;
        private ProcessInfo? _selectedProcess;
        private string _statusMessage = "Application démarrée - Surveillance active";
        private bool _isMonitoring = true;
        private bool _isOptimizing = false;
        private OptimizationMode _currentOptimizationMode = OptimizationMode.None;
        private Visibility _notificationVisibility = Visibility.Collapsed;
        private string _notificationText = string.Empty;
        private int _refreshInterval = 1000;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SystemMonitor SystemMonitor => _systemMonitor;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterProcesses();
            }
        }

        public ObservableCollection<ProcessInfo> FilteredProcesses { get; } = new();

        public ProcessInfo? SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsMonitoring
        {
            get => _isMonitoring;
            set
            {
                _isMonitoring = value;
                OnPropertyChanged();
            }
        }

        public bool IsOptimizing
        {
            get => _isOptimizing;
            set
            {
                _isOptimizing = value;
                OnPropertyChanged();
            }
        }

        public OptimizationMode CurrentOptimizationMode
        {
            get => _currentOptimizationMode;
            set
            {
                _currentOptimizationMode = value;
                OnPropertyChanged();
            }
        }

        public Visibility NotificationVisibility
        {
            get => _notificationVisibility;
            set
            {
                _notificationVisibility = value;
                OnPropertyChanged();
            }
        }

        public string NotificationText
        {
            get => _notificationText;
            set
            {
                _notificationText = value;
                OnPropertyChanged();
            }
        }

        public int RefreshInterval
        {
            get => _refreshInterval;
            set
            {
                _refreshInterval = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartMonitoringCommand { get; }
        public ICommand StopMonitoringCommand { get; }
        public ICommand KillProcessCommand { get; }
        public ICommand EndTaskCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OptimizeForGamingCommand { get; }
        public ICommand OptimizeForProductivityCommand { get; }
        public ICommand OptimizeForBatteryCommand { get; }
        public ICommand CleanMemoryCommand { get; }
        public ICommand ShowAllProcessesCommand { get; }
        public ICommand ShowSystemProcessesCommand { get; }
        public ICommand ShowUserProcessesCommand { get; }

        public MainViewModel()
        {
            _systemMonitor = new SystemMonitor();
            _systemMonitor.PropertyChanged += SystemMonitor_PropertyChanged;

            StartMonitoringCommand = new RelayCommand(StartMonitoring);
            StopMonitoringCommand = new RelayCommand(StopMonitoring);
            KillProcessCommand = new RelayCommand(KillProcess, CanExecuteProcessAction);
            EndTaskCommand = new RelayCommand(EndTask, CanExecuteProcessAction);
            RefreshCommand = new RelayCommand(Refresh);
            OptimizeForGamingCommand = new RelayCommand(() => ApplyOptimization(OptimizationMode.Gaming));
            OptimizeForProductivityCommand = new RelayCommand(() => ApplyOptimization(OptimizationMode.Productivity));
            OptimizeForBatteryCommand = new RelayCommand(() => ApplyOptimization(OptimizationMode.BatterySaving));
            CleanMemoryCommand = new RelayCommand(CleanMemory);
            ShowAllProcessesCommand = new RelayCommand(() => FilterProcesses());
            ShowSystemProcessesCommand = new RelayCommand(ShowSystemProcesses);
            ShowUserProcessesCommand = new RelayCommand(ShowUserProcesses);

            Task.Run(() => UpdateProcesses());
        }

        private void SystemMonitor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SystemMonitor.Processes))
            {
                UpdateProcesses();
            }
        }

        private void UpdateProcesses()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var allProcesses = _systemMonitor.Processes;
                    if (string.IsNullOrWhiteSpace(SearchText))
                    {
                        FilteredProcesses.Clear();
                        foreach (var process in allProcesses)
                        {
                            FilteredProcesses.Add(process);
                        }
                    }
                    else
                    {
                        FilterProcesses();
                    }
                }
                catch { }
            });
        }

        private void FilterProcesses()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var allProcesses = _systemMonitor.Processes;
                    var query = allProcesses.AsEnumerable();

                    if (!string.IsNullOrWhiteSpace(SearchText))
                    {
                        query = query.Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                    }

                    FilteredProcesses.Clear();
                    foreach (var process in query)
                    {
                        FilteredProcesses.Add(process);
                    }
                }
                catch { }
            });
        }

        private void ShowSystemProcesses()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var systemProcesses = _systemMonitor.Processes
                        .Where(p => IsSystemProcess(p))
                        .ToList();

                    FilteredProcesses.Clear();
                    foreach (var process in systemProcesses)
                    {
                        FilteredProcesses.Add(process);
                    }
                }
                catch { }
            });
        }

        private void ShowUserProcesses()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var userProcesses = _systemMonitor.Processes
                        .Where(p => !IsSystemProcess(p))
                        .ToList();

                    FilteredProcesses.Clear();
                    foreach (var process in userProcesses)
                    {
                        FilteredProcesses.Add(process);
                    }
                }
                catch { }
            });
        }

        private bool IsSystemProcess(ProcessInfo process)
        {
            var systemProcesses = new List<string>
            {
                "svchost", "lsass", "csrss", "wininit", "services", "winlogon",
                "explorer", "dwm", "system", "registry", "smss", "conhost",
                "taskhostw", "taskeng", "dllhost", "runtimebroker", "sihost"
            };
            return systemProcesses.Any(p => process.Name.Equals(p, StringComparison.OrdinalIgnoreCase));
        }

        private void StartMonitoring()
        {
            IsMonitoring = true;
            StatusMessage = "Surveillance active";
            ShowNotification("Surveillance démarrée");
        }

        private void StopMonitoring()
        {
            IsMonitoring = false;
            StatusMessage = "Surveillance arrêtée";
            ShowNotification("Surveillance arrêtée");
        }

        private void KillProcess()
        {
            if (SelectedProcess != null)
            {
                try
                {
                    PerformanceOptimizer.KillProcess(SelectedProcess.Id);
                    StatusMessage = $"Processus {SelectedProcess.Name} (ID: {SelectedProcess.Id}) arrêté";
                    ShowNotification($"Processus arrêté: {SelectedProcess.Name}");
                    Task.Run(() => UpdateProcesses());
                }
                catch (Exception ex)
                {
                    ShowNotification($"Erreur: {ex.Message}");
                }
            }
        }

        private void EndTask()
        {
            if (SelectedProcess != null)
            {
                try
                {
                    PerformanceOptimizer.EndTask(SelectedProcess.Id);
                    StatusMessage = $"Processus {SelectedProcess.Name} (ID: {SelectedProcess.Id}) fermé proprement";
                    ShowNotification($"Processus fermé: {SelectedProcess.Name}");
                    Task.Run(() => UpdateProcesses());
                }
                catch (Exception ex)
                {
                    ShowNotification($"Erreur: {ex.Message}");
                }
            }
        }

        private void Refresh()
        {
            Task.Run(() => UpdateProcesses());
            ShowNotification("Rafraîchissement des données");
        }

        private void ApplyOptimization(OptimizationMode mode)
        {
            IsOptimizing = true;
            CurrentOptimizationMode = mode;
            StatusMessage = $"Optimisation en cours: {mode}";

            Task.Run(() =>
            {
                try
                {
                    switch (mode)
                    {
                        case OptimizationMode.Gaming:
                            PerformanceOptimizer.OptimizeCpuForGaming();
                            Application.Current.Dispatcher.Invoke(() => ShowNotification("Optimisation pour le gaming appliquée"));
                            break;
                        case OptimizationMode.Productivity:
                            PerformanceOptimizer.OptimizeCpuForProductivity();
                            Application.Current.Dispatcher.Invoke(() => ShowNotification("Optimisation pour la productivité appliquée"));
                            break;
                        case OptimizationMode.BatterySaving:
                            PerformanceOptimizer.OptimizeCpuForBatterySaving();
                            Application.Current.Dispatcher.Invoke(() => ShowNotification("Optimisation pour l'économie de batterie appliquée"));
                            break;
                    }
                    StatusMessage = $"Optimisation terminée: {mode}";
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => ShowNotification($"Erreur d'optimisation: {ex.Message}"));
                }
                finally
                {
                    IsOptimizing = false;
                }
            });
        }

        private void CleanMemory()
        {
            try
            {
                PerformanceOptimizer.CleanMemory();
                ShowNotification("Mémoire nettoyée");
                StatusMessage = "Mémoire nettoyée";
            }
            catch (Exception ex)
            {
                ShowNotification($"Erreur de nettoyage: {ex.Message}");
            }
        }

        private bool CanExecuteProcessAction()
        {
            return SelectedProcess != null;
        }

        private void ShowNotification(string message)
        {
            NotificationText = message;
            NotificationVisibility = Visibility.Visible;

            Task.Delay(3000).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    NotificationVisibility = Visibility.Collapsed;
                });
            });
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
