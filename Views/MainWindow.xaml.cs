using System;
using System.Windows;
using System.Windows.Controls;
using ProcessManagerApp.Models;
using ProcessManagerApp.ViewModels;

namespace ProcessManagerApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialiser les filtres de catégorie
            CategoryFilterComboBox.Items.Add(new ComboBoxItem { Content = "Toutes les catégories", Tag = null });
            foreach (ProcessCategory category in Enum.GetValues(typeof(ProcessCategory)))
            {
                CategoryFilterComboBox.Items.Add(new ComboBoxItem { Content = category.ToString(), Tag = category });
            }

            // Initialiser les filtres de priorité
            PriorityFilterComboBox.Items.Add(new ComboBoxItem { Content = "Toutes les priorités", Tag = null });
            foreach (ProcessPriority priority in Enum.GetValues(typeof(ProcessPriority)))
            {
                PriorityFilterComboBox.Items.Add(new ComboBoxItem { Content = priority.ToString(), Tag = priority });
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                _viewModel.SearchQuery = textBox.Text;
            }
        }

        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedItem = comboBox.SelectedItem as ComboBoxItem;
                _viewModel.SelectedCategory = selectedItem?.Tag as ProcessCategory?;
            }
        }

        private void PriorityFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedItem = comboBox.SelectedItem as ComboBoxItem;
                _viewModel.SelectedPriority = selectedItem?.Tag as ProcessPriority?;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RefreshProcesses();
        }

        private void ProcessDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ProcessDataGrid.SelectedItem is ProcessInfo selectedProcess)
            {
                _viewModel.ShowProcessDetails(selectedProcess);
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.DataContext is ProcessInfo processInfo)
            {
                _viewModel.ShowProcessDetails(processInfo);
            }
        }

        private void KillButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.DataContext is ProcessInfo processInfo)
            {
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir arrêter le processus '{processInfo.ProcessName}' (ID: {processInfo.ProcessId})?",
                    "Confirmer l'arrêt du processus",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _viewModel.KillProcess(processInfo.ProcessId);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _viewModel.Dispose();
            base.OnClosed(e);
        }
    }
}
