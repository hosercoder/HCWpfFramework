using HCWpfFramework.Models;

namespace HCWpfFramework.Interfaces
{
    /// <summary>
    /// Interface for theme providers that can extend the base theming system
    /// Users can implement this to add custom themes
    /// </summary>
    public interface IThemeProvider
    {
        /// <summary>
        /// Gets the unique identifier for this theme provider
        /// </summary>
        string ProviderId { get; }
        
        /// <summary>
        /// Gets the display name for this theme provider
        /// </summary>
        string DisplayName { get; }
        
        /// <summary>
        /// Gets all themes provided by this provider
        /// </summary>
        /// <returns>Collection of themes</returns>
        IEnumerable<Theme> GetThemes();
        
        /// <summary>
        /// Create a theme with the specified base and customizations
        /// </summary>
        /// <param name="baseTheme">Base theme to extend</param>
        /// <param name="customizations">Custom resource overrides</param>
        /// <returns>Customized theme</returns>
        Theme CreateCustomTheme(Theme baseTheme, Dictionary<string, object> customizations);
        
        /// <summary>
        /// Validate if a theme is compatible with this provider
        /// </summary>
        /// <param name="theme">Theme to validate</param>
        /// <returns>True if compatible</returns>
        bool IsThemeCompatible(Theme theme);
    }

    /// <summary>
    /// Enhanced theme service that supports extensible themes
    /// </summary>
    public interface IExtendedThemeService : IThemeService
    {
        /// <summary>
        /// Register a theme provider
        /// </summary>
        /// <param name="provider">The theme provider to register</param>
        void RegisterThemeProvider(IThemeProvider provider);
        
        /// <summary>
        /// Get all registered theme providers
        /// </summary>
        /// <returns>Collection of theme providers</returns>
        IEnumerable<IThemeProvider> GetThemeProviders();
        
        /// <summary>
        /// Create a custom theme based on an existing theme
        /// </summary>
        /// <param name="baseThemeType">Base theme to extend</param>
        /// <param name="customizations">Custom resource overrides</param>
        /// <param name="customName">Name for the custom theme</param>
        /// <returns>Custom theme</returns>
        Theme CreateCustomTheme(ThemeType baseThemeType, Dictionary<string, object> customizations, string customName);
        
        /// <summary>
        /// Save custom theme for future use
        /// </summary>
        /// <param name="theme">Theme to save</param>
        void SaveCustomTheme(Theme theme);
        
        /// <summary>
        /// Load saved custom themes
        /// </summary>
        /// <returns>Collection of saved custom themes</returns>
        IEnumerable<Theme> LoadCustomThemes();
        
        /// <summary>
        /// Delete a custom theme
        /// </summary>
        /// <param name="themeId">ID of theme to delete</param>
        void DeleteCustomTheme(string themeId);
    }
}