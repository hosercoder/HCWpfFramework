using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using HCWpfFramework.Models;
using HCWpfFramework.ViewModels;

namespace HCWpfFramework.Views
{
    /// <summary>
    /// Complete framework-provided main window with all standard functionality
    /// Applications can use this directly or inherit from it for customizations
    /// </summary>
    public partial class FrameworkMainWindow : Window
    {
        protected MainViewModelBase? ViewModel { get; private set; }

        public FrameworkMainWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        /// <summary>
        /// Constructor for dependency injection
        /// </summary>
        /// <param name="viewModel">The ViewModel to bind to this window</param>
        public FrameworkMainWindow(MainViewModelBase viewModel) : this()
        {
            ViewModel = viewModel;
            DataContext = viewModel;
        }

        /// <summary>
        /// Virtual method to allow derived classes to provide their own ViewModel
        /// </summary>
        /// <param name="serviceProvider">Service provider for DI</param>
        /// <returns>The ViewModel instance</returns>
        protected virtual MainViewModelBase? CreateViewModel(IServiceProvider serviceProvider)
        {
            // Default implementation returns null - should be overridden by derived classes
            // or ViewModel should be provided via constructor
            return null;
        }

        /// <summary>
        /// Virtual method called after ViewModel is set
        /// </summary>
        protected virtual void OnViewModelSet()
        {
            // Default implementation - can be overridden by derived classes
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                // Try to get ViewModel through service provider
                if (Application.Current is IServiceProviderAccessor serviceProviderApp)
                {
                    ViewModel = CreateViewModel(serviceProviderApp.ServiceProvider);
                    if (ViewModel != null)
                    {
                        DataContext = ViewModel;
                        OnViewModelSet();
                    }
                }
            }
        }

        private void OnDockingPanel_WindowFloatRequested(object sender, DockableWindow e)
        {
            ViewModel?.OnWindowFloatRequested(sender, e);
        }

        private void OnDockingPanel_WindowCloseRequested(object sender, DockableWindow e)
        {
            ViewModel?.OnWindowCloseRequested(sender, e);
        }

        protected override void OnClosed(EventArgs e)
        {
            ViewModel?.Dispose();
            base.OnClosed(e);
        }
    }
}