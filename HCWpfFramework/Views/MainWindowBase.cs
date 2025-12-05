using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using HCWpfFramework.Models;
using HCWpfFramework.ViewModels;

namespace HCWpfFramework.Views
{
    /// <summary>
    /// Base class for main application windows that provides framework integration
    /// </summary>
    public abstract class MainWindowBase : Window
    {
        protected MainViewModelBase? ViewModel { get; private set; }

        protected MainWindowBase()
        {
            // Apply framework styling
            Style = (Style)FindResource("ThemedWindowStyle");
            
            // Set default properties
            Height = 600;
            Width = 900;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // Subscribe to loaded event to initialize
            Loaded += OnWindowLoaded;
        }

        /// <summary>
        /// Override this method to set the specific ViewModel type
        /// </summary>
        /// <param name="serviceProvider">The service provider for DI</param>
        /// <returns>The ViewModel instance</returns>
        protected abstract MainViewModelBase CreateViewModel(IServiceProvider serviceProvider);

        /// <summary>
        /// Override this method to customize the window after ViewModel is set
        /// </summary>
        protected virtual void OnViewModelSet()
        {
            // Default implementation - can be overridden
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // This will be set by the derived class or through dependency injection
            if (ViewModel == null && Application.Current is IServiceProviderAccessor serviceProviderApp)
            {
                ViewModel = CreateViewModel(serviceProviderApp.ServiceProvider);
                DataContext = ViewModel;
                OnViewModelSet();
            }
        }

        protected void OnDockingPanel_WindowFloatRequested(object sender, DockableWindow e)
        {
            ViewModel?.OnWindowFloatRequested(sender, e);
        }

        protected void OnDockingPanel_WindowCloseRequested(object sender, DockableWindow e)
        {
            ViewModel?.OnWindowCloseRequested(sender, e);
        }

        protected override void OnClosed(EventArgs e)
        {
            ViewModel?.Dispose();
            base.OnClosed(e);
        }
    }

    /// <summary>
    /// Interface to access ServiceProvider from Application
    /// </summary>
    public interface IServiceProviderAccessor
    {
        IServiceProvider ServiceProvider { get; }
    }
    
    /// <summary>
    /// Helper class to access framework services from anywhere
    /// </summary>
    public static class FrameworkServices
    {
        /// <summary>
        /// Get service provider from the current application
        /// </summary>
        /// <returns>Service provider or null if not available</returns>
        public static IServiceProvider? GetServiceProvider()
        {
            return HCWpfApplication.GetServiceProvider();
        }
        
        /// <summary>
        /// Get a service of type T from the framework
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>Service instance or default if not found</returns>
        public static T? GetService<T>() where T : class
        {
            var serviceProvider = GetServiceProvider();
            return serviceProvider?.GetService<T>();
        }
    }
}