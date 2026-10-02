using System;
using System.Windows;

namespace ProcessManagerApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var mainWindow = new Views.MainWindow();
                var mainViewModel = new ViewModels.MainViewModel();
                mainWindow.DataContext = mainViewModel;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur au démarrage: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
