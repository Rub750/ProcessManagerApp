using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ProcessManagerApp.Models;
using ProcessManagerApp.Services;

namespace ProcessManagerApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ProcessManager _processManager;
        private string _searchQuery;
        private ProcessCategory? _selectedCategory;
        private ProcessPriority? _selectedPriority;
        private ProcessInfo _selectedProcess;
        private string _statusMessage;
        private DateTime _lastRefreshTime;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<ProcessInfo> Processes => _processManager.Processes;

        private ObservableCollection<ProcessInfo> _filteredProcesses = new ObservableCollection<ProcessInfo>();
        public ObservableCollection<ProcessInfo> FilteredProcesses
        {
            get => _filteredProcesses;
            set
            {
                _filteredProcesses = value;
                OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ProcessCategory? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ProcessPriority? SelectedPriority
        {
            get => _selectedPriority;
            set
            {
                _selectedPriority = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ProcessInfo SelectedProcess
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

        public DateTime LastRefreshTime
        {
            get => _lastRefreshTime;
            set
            {
                _lastRefreshTime = value;
                OnPropertyChanged();
            }
        }

        public int ProcessCount => Processes.Count;

        public IEnumerable<ProcessCategory> Categories => Enum.GetValues(typeof(ProcessCategory)).Cast<ProcessCategory>();

        public IEnumerable<ProcessPriority> Priorities => Enum.GetValues(typeof(ProcessPriority)).Cast<ProcessPriority>();

        public MainViewModel()
        {
            _processManager = new ProcessManager();
            _processManager.ProcessAdded += OnProcessAdded;
            _processManager.ProcessRemoved += OnProcessRemoved;
            _processManager.ProcessesUpdated += OnProcessesUpdated;

            // Initialiser les processus
            InitializeProcesses();

            // Mettre à jour le temps de la dernière mise à jour
            LastRefreshTime = DateTime.Now;
            StatusMessage = "Prêt";
        }

        private async void InitializeProcesses()
        {
            try
            {
                StatusMessage = "Chargement des processus...";
                await _processManager.RefreshProcessesAsync();
                ApplyFilters();
                StatusMessage = "Processus chargés avec succès";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors du chargement: {ex.Message}";
            }
        }

        private void OnProcessAdded(object sender, ProcessInfo process)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LastRefreshTime = DateTime.Now;
                OnPropertyChanged(nameof(ProcessCount));
            });
        }

        private void OnProcessRemoved(object sender, ProcessInfo process)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LastRefreshTime = DateTime.Now;
                OnPropertyChanged(nameof(ProcessCount));
            });
        }

        private void OnProcessesUpdated(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LastRefreshTime = DateTime.Now;
                ApplyFilters();
                OnPropertyChanged(nameof(ProcessCount));
            });
        }

        private void ApplyFilters()
        {
            try
            {
                var filtered = _processManager.Processes.AsEnumerable();

                // Appliquer le filtre de recherche
                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    var query = SearchQuery.ToLower();
                    filtered = filtered.Where(p =>
                        p.ProcessName.ToLower().Contains(query) ||
                        p.WindowTitle.ToLower().Contains(query) ||
                        (p.ExecutablePath != null && p.ExecutablePath.ToLower().Contains(query)));
                }

                // Appliquer le filtre de catégorie
                if (SelectedCategory.HasValue)
                {
                    filtered = filtered.Where(p => p.Category == SelectedCategory.Value);
                }

                // Appliquer le filtre de priorité
                if (SelectedPriority.HasValue)
                {
                    filtered = filtered.Where(p => p.Priority == SelectedPriority.Value);
                }

                // Mettre à jour la collection filtrée
                FilteredProcesses = new ObservableCollection<ProcessInfo>(filtered);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors du filtrage: {ex.Message}";
            }
        }

        public async void RefreshProcesses()
        {
            try
            {
                StatusMessage = "Rafraîchissement des processus...";
                await _processManager.RefreshProcessesAsync();
                LastRefreshTime = DateTime.Now;
                StatusMessage = "Processus rafraîchis avec succès";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors du rafraîchissement: {ex.Message}";
            }
        }

        public async void KillProcess(int processId)
        {
            try
            {
                var processInfo = Processes.FirstOrDefault(p => p.ProcessId == processId);
                if (processInfo == null)
                {
                    StatusMessage = "Processus introuvable";
                    return;
                }

                if (processInfo.IsCritical)
                {
                    StatusMessage = "Impossible de terminer un processus critique pour le système";
                    return;
                }

                StatusMessage = $"Arrêt du processus {processInfo.ProcessName}...";
                await _processManager.KillProcessAsync(processId);
                StatusMessage = $"Processus {processInfo.ProcessName} arrêté avec succès";
            }
            catch (InvalidOperationException ex)
            {
                StatusMessage = ex.Message;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de l'arrêt du processus: {ex.Message}";
            }
        }

        public async void ChangeProcessPriority(int processId, ProcessPriority priority)
        {
            try
            {
                StatusMessage = "Changement de priorité...";
                await _processManager.ChangeProcessPriorityAsync(processId, priority);
                StatusMessage = "Priorité changée avec succès";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors du changement de priorité: {ex.Message}";
            }
        }

        public void ShowProcessDetails(ProcessInfo processInfo)
        {
            if (processInfo == null)
                return;

            var message = $"Détails du processus:\n\n" +
                          $"ID: {processInfo.ProcessId}\n" +
                          $"Nom: {processInfo.ProcessName}\n" +
                          $"Titre de la fenêtre: {processInfo.WindowTitle}\n" +
                          $"Chemin: {processInfo.ExecutablePath}\n" +
                          $"Catégorie: {processInfo.CategoryString}\n" +
                          $"Priorité: {processInfo.PriorityString}\n" +
                          $"CPU: {processInfo.CpuUsagePercent}\n" +
                          $"Mémoire: {processInfo.MemoryUsageMB}\n" +
                          $"Temps d'activité: {processInfo.UptimeString}\n" +
                          $"Threads: {processInfo.ThreadCount}\n" +
                          $"Handles: {processInfo.HandleCount}\n" +
                          $"Processus critique: {(processInfo.IsCritical ? "Oui" : "Non")}";

            MessageBox.Show(message, "Détails du processus", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Dispose()
        {
            _processManager?.Dispose();
        }
    }
}
