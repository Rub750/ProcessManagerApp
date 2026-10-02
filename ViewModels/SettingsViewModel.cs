using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ProcessManagerApp.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool _autoStartMonitoring = true;
        private bool _showNotifications = true;
        private int _refreshInterval = 1000;
        private bool _enableOptimization = true;
        private bool _darkMode = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool AutoStartMonitoring
        {
            get => _autoStartMonitoring;
            set
            {
                _autoStartMonitoring = value;
                OnPropertyChanged();
            }
        }

        public bool ShowNotifications
        {
            get => _showNotifications;
            set
            {
                _showNotifications = value;
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

        public bool EnableOptimization
        {
            get => _enableOptimization;
            set
            {
                _enableOptimization = value;
                OnPropertyChanged();
            }
        }

        public bool DarkMode
        {
            get => _darkMode;
            set
            {
                _darkMode = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveSettingsCommand { get; }
        public ICommand ResetSettingsCommand { get; }

        public SettingsViewModel()
        {
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            ResetSettingsCommand = new RelayCommand(ResetSettings);
        }

        private void SaveSettings()
        {
            try
            {
                Properties.Settings.Default.AutoStartMonitoring = AutoStartMonitoring;
                Properties.Settings.Default.ShowNotifications = ShowNotifications;
                Properties.Settings.Default.RefreshInterval = RefreshInterval;
                Properties.Settings.Default.EnableOptimization = EnableOptimization;
                Properties.Settings.Default.DarkMode = DarkMode;
                Properties.Settings.Default.Save();

                MessageBox.Show("Paramètres enregistrés avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetSettings()
        {
            AutoStartMonitoring = true;
            ShowNotifications = true;
            RefreshInterval = 1000;
            EnableOptimization = true;
            DarkMode = true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
