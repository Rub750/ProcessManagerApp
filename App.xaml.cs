using System;
using System.Windows;

namespace ProcessManagerApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Gérer les exceptions non gérées
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Une erreur inattendue s'est produite:\n\n{e.Exception.Message}", 
                          "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                MessageBox.Show($"Une erreur critique s'est produite:\n\n{exception.Message}", 
                              "Erreur Critique", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
