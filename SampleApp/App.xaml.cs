using System.Windows;
using HCWpfFramework;
using SampleApp.ViewModels;
using System.Diagnostics;

namespace SampleApp
{
    /// <summary>
    /// The simplest possible App.xaml.cs using the framework
    /// This demonstrates the ultimate goal - minimal user code!
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Debug.WriteLine("Starting HCWpfFramework application...");
                
                // Call base first to initialize the application
                base.OnStartup(e);
                
                // One method call - framework handles everything!
                var mainWindow = HCWpfApplication.RunApplication<UltimateSimpleViewModel>();
                
                Debug.WriteLine("Application started successfully!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Application startup failed: {ex}");
                var message = $"Application startup failed:\n\nError: {ex.Message}\n\nDetails: {ex.GetType().Name}";
                if (ex.InnerException != null)
                {
                    message += $"\n\nInner Exception: {ex.InnerException.Message}";
                }
                MessageBox.Show(message, "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }
    }
}