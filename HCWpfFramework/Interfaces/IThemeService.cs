using HCWpfFramework.Models;

namespace HCWpfFramework.Interfaces
{
    public interface IThemeService
    {
        Theme CurrentTheme { get; }
        IEnumerable<Theme> AvailableThemes { get; }
        event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
        
        void SetTheme(ThemeType themeType);
        void SetTheme(Theme theme);
        Theme GetTheme(ThemeType themeType);
        void RegisterTheme(Theme theme);
        void SaveThemePreference();
        void LoadThemePreference();
    }
}