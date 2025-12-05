using System.Windows;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;
using HCWpfFramework.Interfaces;
using HCWpfFramework.Models;

namespace HCWpfFramework.Services
{
    public class ThemeService : IThemeService
    {
        private readonly Dictionary<ThemeType, Theme> _themes = new();
        private Theme _currentTheme;
        private const string THEME_PREFERENCE_KEY = "HCWpfFrameword_Theme";

        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

        public Theme CurrentTheme => _currentTheme;
        public IEnumerable<Theme> AvailableThemes => _themes.Values;

        public ThemeService()
        {
            InitializeDefaultThemes();
            _currentTheme = _themes[ThemeType.Light]; // Default to light theme
            LoadThemePreference();
        }

        private void InitializeDefaultThemes()
        {
            // Light Theme
            var lightTheme = new Theme
            {
                Name = "Light",
                Type = ThemeType.Light,
                Resources = new Dictionary<string, object>
                {
                    // Background colors
                    ["PrimaryBackgroundBrush"] = "#FFFFFF",
                    ["SecondaryBackgroundBrush"] = "#F5F5F5",
                    ["TertiaryBackgroundBrush"] = "#E9ECEF",
                    
                    // Text colors
                    ["PrimaryTextBrush"] = "#212529",
                    ["SecondaryTextBrush"] = "#6C757D",
                    ["AccentTextBrush"] = "#0D6EFD",
                    
                    // Border colors
                    ["BorderBrush"] = "#DEE2E6",
                    ["FocusBorderBrush"] = "#0D6EFD",
                    ["HoverBorderBrush"] = "#6C757D",
                    
                    // Button colors
                    ["ButtonBackgroundBrush"] = "#F8F9FA",
                    ["ButtonHoverBackgroundBrush"] = "#E9ECEF",
                    ["ButtonPressedBackgroundBrush"] = "#DEE2E6",
                    ["ButtonTextBrush"] = "#212529",
                    
                    // Menu colors
                    ["MenuBackgroundBrush"] = "#FFFFFF",
                    ["MenuHoverBackgroundBrush"] = "#E9ECEF",
                    ["MenuTextBrush"] = "#212529",
                    
                    // StatusBar colors
                    ["StatusBarBackgroundBrush"] = "#F8F9FA",
                    ["StatusBarTextBrush"] = "#6C757D",
                    
                    // Docking panel colors
                    ["DockingPanelHeaderBrush"] = "#E9ECEF",
                    ["DockingPanelContentBrush"] = "#FFFFFF",
                    ["DropZoneBackgroundBrush"] = "#CCE5FF",
                    ["DropZoneBorderBrush"] = "#0D6EFD",
                    
                    // Tab colors
                    ["TabBackgroundBrush"] = "#F8F9FA",
                    ["TabSelectedBackgroundBrush"] = "#FFFFFF",
                    ["TabHoverBackgroundBrush"] = "#E9ECEF",
                    ["TabTextBrush"] = "#495057",
                    ["TabSelectedTextBrush"] = "#212529",
                    
                    // Splitter colors
                    ["SplitterBrush"] = "#DEE2E6",
                    ["SplitterHoverBrush"] = "#ADB5BD",
                    
                    // Scrollbar colors
                    ["ScrollBarBackgroundBrush"] = "#F8F9FA",
                    ["ScrollBarThumbBrush"] = "#ADB5BD",
                    ["ScrollBarThumbHoverBrush"] = "#6C757D"
                }
            };

            // Dark Theme
            var darkTheme = new Theme
            {
                Name = "Dark",
                Type = ThemeType.Dark,
                Resources = new Dictionary<string, object>
                {
                    // Background colors
                    ["PrimaryBackgroundBrush"] = "#1E1E1E",
                    ["SecondaryBackgroundBrush"] = "#252526",
                    ["TertiaryBackgroundBrush"] = "#2D2D30",
                    
                    // Text colors
                    ["PrimaryTextBrush"] = "#FFFFFF",
                    ["SecondaryTextBrush"] = "#CCCCCC",
                    ["AccentTextBrush"] = "#569CD6",
                    
                    // Border colors
                    ["BorderBrush"] = "#3C3C3C",
                    ["FocusBorderBrush"] = "#569CD6",
                    ["HoverBorderBrush"] = "#505050",
                    
                    // Button colors
                    ["ButtonBackgroundBrush"] = "#333333",
                    ["ButtonHoverBackgroundBrush"] = "#404040",
                    ["ButtonPressedBackgroundBrush"] = "#505050",
                    ["ButtonTextBrush"] = "#FFFFFF",
                    
                    // Menu colors
                    ["MenuBackgroundBrush"] = "#2D2D30",
                    ["MenuHoverBackgroundBrush"] = "#3E3E40",
                    ["MenuTextBrush"] = "#FFFFFF",
                    
                    // StatusBar colors
                    ["StatusBarBackgroundBrush"] = "#007ACC",
                    ["StatusBarTextBrush"] = "#FFFFFF",
                    
                    // Docking panel colors
                    ["DockingPanelHeaderBrush"] = "#2D2D30",
                    ["DockingPanelContentBrush"] = "#1E1E1E",
                    ["DropZoneBackgroundBrush"] = "#264F78",
                    ["DropZoneBorderBrush"] = "#569CD6",
                    
                    // Tab colors
                    ["TabBackgroundBrush"] = "#2D2D30",
                    ["TabSelectedBackgroundBrush"] = "#1E1E1E",
                    ["TabHoverBackgroundBrush"] = "#3E3E40",
                    ["TabTextBrush"] = "#CCCCCC",
                    ["TabSelectedTextBrush"] = "#FFFFFF",
                    
                    // Splitter colors
                    ["SplitterBrush"] = "#3C3C3C",
                    ["SplitterHoverBrush"] = "#505050",
                    
                    // Scrollbar colors
                    ["ScrollBarBackgroundBrush"] = "#2D2D30",
                    ["ScrollBarThumbBrush"] = "#686868",
                    ["ScrollBarThumbHoverBrush"] = "#9E9E9E"
                }
            };

            RegisterTheme(lightTheme);
            RegisterTheme(darkTheme);
        }

        public void SetTheme(ThemeType themeType)
        {
            if (_themes.TryGetValue(themeType, out var theme))
            {
                SetTheme(theme);
            }
        }

        public void SetTheme(Theme theme)
        {
            if (theme == null) return;

            var oldTheme = _currentTheme;
            _currentTheme = theme;

            ApplyThemeToApplication(theme);
            OnThemeChanged(new ThemeChangedEventArgs(theme, oldTheme));
            SaveThemePreference();
        }

        private void ApplyThemeToApplication(Theme theme)
        {
            var app = Application.Current;
            if (app?.Resources == null) return;

            // Clear existing theme resources
            var keysToRemove = app.Resources.Keys.Cast<object>()
                .Where(key => key.ToString()?.EndsWith("Brush") == true)
                .ToList();

            foreach (var key in keysToRemove)
            {
                if (app.Resources.Contains(key))
                {
                    app.Resources.Remove(key);
                }
            }

            // Apply new theme resources
            foreach (var resource in theme.Resources)
            {
                try
                {
                    // Convert color strings to SolidColorBrush
                    if (resource.Value is string colorString && colorString.StartsWith("#"))
                    {
                        var converter = new System.Windows.Media.BrushConverter();
                        var brush = converter.ConvertFromString(colorString);
                        if (brush != null)
                        {
                            app.Resources[resource.Key] = brush;
                        }
                    }
                    else
                    {
                        app.Resources[resource.Key] = resource.Value;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to apply theme resource {resource.Key}: {ex.Message}");
                }
            }

            // Force refresh of all windows
            foreach (Window window in app.Windows)
            {
                try
                {
                    window.InvalidateVisual();
                    window.UpdateLayout();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to refresh window: {ex.Message}");
                }
            }
        }

        public Theme GetTheme(ThemeType themeType)
        {
            return _themes.TryGetValue(themeType, out var theme) ? theme : _themes[ThemeType.Light];
        }

        public void RegisterTheme(Theme theme)
        {
            _themes[theme.Type] = theme;
        }

        public void SaveThemePreference()
        {
            try
            {
                var settings = new { ThemeType = _currentTheme.Type.ToString() };
                var json = JsonSerializer.Serialize(settings);
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appFolder = Path.Combine(appDataPath, "HCWpfFramework");
                Directory.CreateDirectory(appFolder);
                var settingsFile = Path.Combine(appFolder, "theme-settings.json");
                File.WriteAllText(settingsFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save theme preference: {ex.Message}");
            }
        }

        public void LoadThemePreference()
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var settingsFile = Path.Combine(appDataPath, "HCWpfFramework", "theme-settings.json");

                if (File.Exists(settingsFile))
                {
                    var json = File.ReadAllText(settingsFile);
                    var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    
                    if (settings != null && settings.TryGetValue("ThemeType", out var themeTypeObj))
                    {
                        var themeTypeElement = (JsonElement)themeTypeObj;
                        if (Enum.TryParse<ThemeType>(themeTypeElement.GetString(), out var themeType))
                        {
                            SetTheme(themeType);
                            return;
                        }
                    }
                }

                // Check system theme preference as fallback
                if (IsSystemDarkMode())
                {
                    SetTheme(ThemeType.Dark);
                }
                else
                {
                    SetTheme(ThemeType.Light);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load theme preference: {ex.Message}");
                SetTheme(ThemeType.Light);
            }
        }

        private bool IsSystemDarkMode()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = key?.GetValue("AppsUseLightTheme");
                return value is int intValue && intValue == 0;
            }
            catch
            {
                return false;
            }
        }

        private void OnThemeChanged(ThemeChangedEventArgs e)
        {
            ThemeChanged?.Invoke(this, e);
        }
    }
}