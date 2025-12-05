using Microsoft.Extensions.DependencyInjection;
using HCWpfFramework.Interfaces;
using HCWpfFramework.Services;
using HCWpfFramework.ViewModels;

namespace HCWpfFramework.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection to easily register HCWpfFramework services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add all HCWpfFramework services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configureTheme">Optional action to configure the theme service</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddHCWpfFramework(this IServiceCollection services, Action<IThemeService>? configureTheme = null)
        {
            // Register core services
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IMessageService>(provider => ThreadSafeMessageService.Instance);
            services.AddSingleton<IWindowFactoryService, WindowFactoryService>();
            services.AddSingleton<ILayoutService>(provider => 
                new LayoutService(provider.GetService<IWindowFactoryService>()));
            
            // Register background services
            services.AddTransient<BackgroundWorkerService>();
            
            // Configure theme service if action provided
            if (configureTheme != null)
            {
                var serviceProvider = services.BuildServiceProvider();
                var themeService = serviceProvider.GetRequiredService<IThemeService>();
                configureTheme(themeService);
            }
            
            return services;
        }

        /// <summary>
        /// Add theme services only
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="initialTheme">Optional initial theme to apply</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddHCWpfThemeService(this IServiceCollection services, Models.ThemeType? initialTheme = null)
        {
            services.AddSingleton<IThemeService>(provider =>
            {
                var themeService = new ThemeService();
                if (initialTheme.HasValue)
                {
                    themeService.SetTheme(initialTheme.Value);
                }
                return themeService;
            });
            
            return services;
        }

        /// <summary>
        /// Add messaging services only
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddHCWpfMessaging(this IServiceCollection services)
        {
            services.AddSingleton<IMessageService>(ThreadSafeMessageService.Instance);
            return services;
        }

        /// <summary>
        /// Add background worker services
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddHCWpfBackgroundServices(this IServiceCollection services)
        {
            services.AddTransient<BackgroundWorkerService>();
            return services;
        }

        /// <summary>
        /// Add layout services only
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="initialLayout">Optional initial layout to apply</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddHCWpfLayoutService(this IServiceCollection services, Models.LayoutType? initialLayout = null)
        {
            services.AddSingleton<ILayoutService>(provider =>
            {
                var layoutService = new LayoutService();
                if (initialLayout.HasValue)
                {
                    layoutService.SetLayout(initialLayout.Value);
                }
                return layoutService;
            });
            
            return services;
        }

        /// <summary>
        /// Add framework views and windows
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddHCWpfViews(this IServiceCollection services)
        {
            services.AddTransient<Views.FrameworkMainWindow>();
            return services;
        }
        
        /// <summary>
        /// Add view model base classes and helpers
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddHCWpfViewModels(this IServiceCollection services)
        {
            // ViewModelBase is abstract, but we can register factory methods for concrete implementations
            // This method is mainly for consistency and future extensibility
            return services;
        }
    }
}