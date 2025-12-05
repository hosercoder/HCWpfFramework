using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using HCWpfFramework.Interfaces;
using HCWpfFramework.Services;
using HCWpfFramework.Views;
using HCWpfFramework.ViewModels;
using HCWpfFramework.Models;
using HCWpfFramework.Extensions;

namespace HCWpfFramework
{
    /// <summary>
    /// Application framework that handles all the complexity
    /// Users just need to call one method to get a complete application
    /// </summary>
    public static class HCWpfApplication
    {
        /// <summary>
        /// Gets the service provider from the current application
        /// </summary>
        /// <returns>Service provider or null if not found</returns>
        public static IServiceProvider? GetServiceProvider()
        {
            return Application.Current?.Properties["ServiceProvider"] as IServiceProvider;
        }
        
        /// <summary>
        /// Create and run a complete application from Main method (alternative to App.xaml approach)
        /// This creates its own Application instance and is suitable for console app conversion
        /// </summary>
        /// <typeparam name="TViewModel">The main ViewModel type</typeparam>
        /// <param name="configureServices">Optional service configuration</param>
        /// <param name="windowFactoryAssemblies">Assemblies to scan for window factories</param>
        [STAThread]
        public static void CreateAndRunApplication<TViewModel>(
            Action<IServiceCollection>? configureServices = null,
            params Assembly[] windowFactoryAssemblies)
            where TViewModel : MainViewModelBase
        {
            var serviceProvider = CreateApplication<TViewModel>(configureServices, windowFactoryAssemblies);
            
            // Create new application instance (for use without App.xaml)
            var app = new Application();
            
            // Make service provider available
            app.Properties["ServiceProvider"] = serviceProvider;
            
            // Initialize framework services
            var themeService = serviceProvider.GetRequiredService<IThemeService>();
            themeService.LoadThemePreference();
            
            // Create main window
            var viewModel = serviceProvider.GetRequiredService<TViewModel>();
            var mainWindow = new FrameworkMainWindow(viewModel);
            
            // Run application with main window
            app.Run(mainWindow);
        }
        /// <summary>
        /// Create a complete WPF application with minimal setup
        /// This is the main entry point for framework users
        /// </summary>
        /// <typeparam name="TViewModel">The main ViewModel type (must inherit from MainViewModelBase)</typeparam>
        /// <param name="configureServices">Optional service configuration</param>
        /// <param name="windowFactoryAssemblies">Assemblies to scan for window factories</param>
        /// <returns>Service provider for the application</returns>
        public static IServiceProvider CreateApplication<TViewModel>(
            Action<IServiceCollection>? configureServices = null,
            params Assembly[] windowFactoryAssemblies)
            where TViewModel : MainViewModelBase
        {
            // Create service collection with framework services
            var services = new ServiceCollection();
            
            // Register all framework services
            services.AddHCWpfFramework();
            
            // Register the main ViewModel
            services.AddTransient<TViewModel>();
            
            // Allow user to configure additional services
            configureServices?.Invoke(services);
            
            // Build service provider
            var serviceProvider = services.BuildServiceProvider();
            
            // Auto-register window factories from specified assemblies
            var windowFactoryService = serviceProvider.GetRequiredService<IWindowFactoryService>();
            
            // Always scan the calling assembly
            var callingAssembly = Assembly.GetCallingAssembly();
            windowFactoryService.RegisterFactoriesFromAssembly(callingAssembly);
            
            // Scan additional assemblies
            foreach (var assembly in windowFactoryAssemblies)
            {
                windowFactoryService.RegisterFactoriesFromAssembly(assembly);
            }
            
            return serviceProvider;
        }
        
        /// <summary>
        /// Initialize and show a complete WPF application with minimal setup
        /// Works with the existing Application instance
        /// </summary>
        /// <typeparam name="TViewModel">The main ViewModel type</typeparam>
        /// <param name="configureServices">Optional service configuration</param>
        /// <param name="windowFactoryAssemblies">Assemblies to scan for window factories</param>
        /// <returns>The created main window</returns>
        public static Window RunApplication<TViewModel>(
            Action<IServiceCollection>? configureServices = null,
            params Assembly[] windowFactoryAssemblies)
            where TViewModel : MainViewModelBase
        {
            var serviceProvider = CreateApplication<TViewModel>(configureServices, windowFactoryAssemblies);
            
            // Use existing application instance
            var app = Application.Current;
            if (app == null)
            {
                throw new InvalidOperationException("No Application instance found. This method should be called from within a WPF Application.");
            }
            
            // Make service provider available
            app.Properties["ServiceProvider"] = serviceProvider;
            
            // Initialize framework services
            var themeService = serviceProvider.GetRequiredService<IThemeService>();
            themeService.LoadThemePreference();
            
            // Create main window
            var viewModel = serviceProvider.GetRequiredService<TViewModel>();
            var mainWindow = new FrameworkMainWindow(viewModel);
            
            // Show the main window
            mainWindow.Show();
            
            return mainWindow;
        }
        
        /// <summary>
        /// Quick setup for simple applications - just provide window factories
        /// </summary>
        /// <param name="windowFactories">Window factories to register</param>
        /// <returns>The created main window</returns>
        public static Window RunSimpleApplication(params IWindowFactory[] windowFactories)
        {
            var services = new ServiceCollection();
            services.AddHCWpfFramework();
            services.AddTransient<SimpleMainViewModel>();
            
            var serviceProvider = services.BuildServiceProvider();
            var windowFactoryService = serviceProvider.GetRequiredService<IWindowFactoryService>();
            
            // Register provided factories
            foreach (var factory in windowFactories)
            {
                windowFactoryService.RegisterFactory(factory);
            }
            
            return RunApplication<SimpleMainViewModel>();
        }
    }

    /// <summary>
    /// Simple main ViewModel for basic applications
    /// </summary>
    public class SimpleMainViewModel : MainViewModelBase
    {
        public SimpleMainViewModel(IMessageService messageService, IThemeService themeService, ILayoutService layoutService)
            : base(messageService, themeService, layoutService)
        {
            Title = "HCWpfFramework Application";
        }

        protected override void CustomizeLayout()
        {
            // Use all registered window factories
            var windowFactoryService = GetLayoutService as IWindowFactoryService;
            if (windowFactoryService != null)
            {
                var categories = new[] { "General", "Tools", "Views", "Data" };
                foreach (var category in categories)
                {
                    var factories = windowFactoryService.GetFactoriesByCategory(category);
                    foreach (var factory in factories)
                    {
                        var window = windowFactoryService.CreateWindow(factory.WindowId);
                        if (window != null)
                        {
                            AddWindowToArea(window, factory.DefaultDockingArea);
                        }
                    }
                }
            }
        }
        
        private void AddWindowToArea(Models.DockableWindow window, DockingArea area)
        {
            switch (area)
            {
                case DockingArea.Left:
                    LeftPanelWindows.Add(window);
                    break;
                case DockingArea.Right:
                    RightPanelWindows.Add(window);
                    break;
                case DockingArea.Center:
                    CenterPanelWindows.Add(window);
                    break;
                case DockingArea.Bottom:
                    BottomPanelWindows.Add(window);
                    break;
                case DockingArea.Top:
                    TopPanelWindows.Add(window);
                    break;
            }
        }
    }
}