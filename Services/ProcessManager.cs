using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ProcessManagerApp.Models;

namespace ProcessManagerApp.Services
{
    public class ProcessManager : IDisposable
    {
        private readonly ObservableCollection<ProcessInfo> _processes = new ObservableCollection<ProcessInfo>();
        private System.Timers.Timer _refreshTimer;
        private bool _isRefreshing = false;

        public event EventHandler<ProcessInfo> ProcessAdded;
        public event EventHandler<ProcessInfo> ProcessRemoved;
        public event EventHandler ProcessesUpdated;

        public ObservableCollection<ProcessInfo> Processes => _processes;

        public ProcessManager()
        {
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _refreshTimer = new System.Timers.Timer(2000); // Rafraîchir toutes les 2 secondes
            _refreshTimer.Elapsed += async (sender, e) => await RefreshProcessesAsync();
            _refreshTimer.AutoReset = true;
            _refreshTimer.Enabled = true;
        }

        public async Task RefreshProcessesAsync()
        {
            if (_isRefreshing)
                return;

            _isRefreshing = true;
            
            try
            {
                var currentProcessIds = new HashSet<int>(_processes.Select(p => p.ProcessId));
                var newProcesses = new List<ProcessInfo>();
                
                // Obtenir tous les processus
                var allProcesses = Process.GetProcesses();
                
                // Traiter chaque processus
                foreach (var process in allProcesses)
                {
                    try
                    {
                        var processInfo = ProcessInfo.FromProcess(process);
                        if (processInfo != null)
                        {
                            newProcesses.Add(processInfo);
                        }
                    }
                    catch
                    {
                        // Ignorer les processus inaccessibles
                    }
                }

                // Mettre à jour la collection
                await UpdateProcessCollectionAsync(newProcesses, currentProcessIds);
                
                ProcessesUpdated?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                // Erreur lors du rafraîchissement
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        private async Task UpdateProcessCollectionAsync(List<ProcessInfo> newProcesses, HashSet<int> currentProcessIds)
        {
            // Utiliser le dispatcher pour mettre à jour l'UI
            await Task.Run(() =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    // Supprimer les processus qui n'existent plus
                    var processesToRemove = _processes.Where(p => !newProcesses.Any(np => np.ProcessId == p.ProcessId)).ToList();
                    foreach (var process in processesToRemove)
                    {
                        _processes.Remove(process);
                        ProcessRemoved?.Invoke(this, process);
                    }

                    // Mettre à jour ou ajouter les nouveaux processus
                    foreach (var newProcess in newProcesses)
                    {
                        var existingProcess = _processes.FirstOrDefault(p => p.ProcessId == newProcess.ProcessId);
                        
                        if (existingProcess != null)
                        {
                            // Mettre à jour les informations du processus existant
                            UpdateProcess(existingProcess, newProcess);
                        }
                        else
                        {
                            // Ajouter le nouveau processus
                            _processes.Add(newProcess);
                            ProcessAdded?.Invoke(this, newProcess);
                        }
                    }
                });
            });
        }

        private void UpdateProcess(ProcessInfo existing, ProcessInfo updated)
        {
            existing.ProcessName = updated.ProcessName;
            existing.WindowTitle = updated.WindowTitle;
            existing.ExecutablePath = updated.ExecutablePath;
            existing.Priority = updated.Priority;
            existing.Category = updated.Category;
            existing.StartTime = updated.StartTime;
            existing.MemoryUsage = updated.MemoryUsage;
            existing.CpuUsage = updated.CpuUsage;
            existing.ThreadCount = updated.ThreadCount;
            existing.HandleCount = updated.HandleCount;
            existing.IsCritical = updated.IsCritical;
            existing.ProcessIcon = updated.ProcessIcon;
        }

        public async Task KillProcessAsync(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                
                // Vérifier si le processus est critique
                var processInfo = _processes.FirstOrDefault(p => p.ProcessId == processId);
                if (processInfo?.IsCritical == true)
                {
                    throw new InvalidOperationException("Impossible de terminer un processus critique pour le système.");
                }

                process.Kill();
                process.WaitForExit();
                
                await Task.Delay(100); // Attendre un peu pour la mise à jour
                await RefreshProcessesAsync();
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la terminaison du processus: " + processId, ex);
            }
        }

        public async Task KillProcessByNameAsync(string processName)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);
                
                foreach (var process in processes)
                {
                    var processInfo = _processes.FirstOrDefault(p => p.ProcessId == process.Id);
                    if (processInfo?.IsCritical == true)
                    {
                        throw new InvalidOperationException("Impossible de terminer un processus critique pour le système.");
                    }

                    process.Kill();
                    process.WaitForExit();
                }
                
                await Task.Delay(100);
                await RefreshProcessesAsync();
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la terminaison du processus: " + processName, ex);
            }
        }


        public async Task ChangeProcessPriorityAsync(int processId, ProcessPriority priority)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                process.PriorityClass = (ProcessPriorityClass)priority;
                await Task.Delay(100);
                await RefreshProcessesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors du changement de priorité: " + processId, ex);
            }
        }
        public void Dispose()
        {
            _refreshTimer?.Dispose();
        }
    }
}
