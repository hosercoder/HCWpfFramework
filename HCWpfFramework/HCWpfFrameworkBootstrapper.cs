using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using HCWpfFramework.Interfaces;
using HCWpfFramework.Services;

namespace HCWpfFramework
{
    /// <summary>
    /// Bootstrapper class to initialize the HCWpfFramework in consuming applications
    /// </summary>
    public static class HCWpfFrameworkBootstrapper
    {
        private static bool _isInitialized = false;
        private static readonly object _lock = new();

        /// <summary>
        /// Initialize the framework with default services
        /// Call this method in your application's App.xaml.cs OnStartup method
        /// </summary>
        /// <param name="serviceCollection">Optional service collection for dependency injection</param>
        public static IServiceCollection Initialize(IServiceCollection? serviceCollection = null)
        {
            lock (_lock)
            {
                if (_isInitialized)
                    return serviceCollection ?? new ServiceCollection();

                var services = serviceCollection ?? new ServiceCollection();
                
                // Register framework services
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<IMessageService>(ThreadSafeMessageService.Instance);
                services.AddTransient<BackgroundWorkerService>();

                // Initialize theme service
                var themeService = new ThemeService();
                
                // Apply the theme service to the application if available
                if (Application.Current != null)
                {
                    InitializeApplicationResources(themeService);
                }

                _isInitialized = true;
                return services;
            }
        }

        /// <summary>
        /// Initialize with a specific theme
        /// </summary>
        /// <param name="initialTheme">The theme to apply on initialization</param>
        /// <param name="serviceCollection">Optional service collection for dependency injection</param>
        public static IServiceCollection Initialize(Models.ThemeType initialTheme, IServiceCollection? serviceCollection = null)
        {
            var services = Initialize(serviceCollection);
            
            var themeService = new ThemeService();
            themeService.SetTheme(initialTheme);
            
            return services;
        }

        /// <summary>
        /// Initialize framework resources in the WPF application
        /// </summary>
        private static void InitializeApplicationResources(IThemeService themeService)
        {
            if (Application.Current?.Resources == null)
                return;

            // Load framework resource dictionaries
            try
            {
                var frameworkResources = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/HCWpfFramework;component/Themes/Generic.xaml", UriKind.Absolute)
                };
                
                Application.Current.Resources.MergedDictionaries.Add(frameworkResources);
            }
            catch
            {
                // Fallback: Apply basic theme resources directly
                themeService.LoadThemePreference();
            }
        }

        /// <summary>
        /// Clean up framework resources
        /// Call this method in your application's App.xaml.cs OnExit method
        /// </summary>
        public static void Cleanup()
        {
            lock (_lock)
            {
                if (!_isInitialized)
                    return;

                ThreadSafeMessageService.Instance.Dispose();
                _isInitialized = false;
            }
        }

        /// <summary>
        /// Check if the framework has been initialized
        /// </summary>
        public static bool IsInitialized => _isInitialized;
    }
}