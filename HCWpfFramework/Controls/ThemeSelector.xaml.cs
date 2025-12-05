using System.Windows;
using System.Windows.Controls;
using HCWpfFramework.Models;
using HCWpfFramework.Interfaces;

namespace HCWpfFramework.Controls
{
    public partial class ThemeSelector : UserControl
    {
        public static readonly DependencyProperty ThemeServiceProperty =
            DependencyProperty.Register(nameof(ThemeService), typeof(IThemeService), typeof(ThemeSelector),
                new PropertyMetadata(null, OnThemeServiceChanged));

        public static readonly DependencyProperty CurrentThemeIndexProperty =
            DependencyProperty.Register(nameof(CurrentThemeIndex), typeof(int), typeof(ThemeSelector),
                new PropertyMetadata(0));

        public IThemeService? ThemeService
        {
            get => (IThemeService?)GetValue(ThemeServiceProperty);
            set => SetValue(ThemeServiceProperty, value);
        }

        public int CurrentThemeIndex
        {
            get => (int)GetValue(CurrentThemeIndexProperty);
            set => SetValue(CurrentThemeIndexProperty, value);
        }

        public ThemeSelector()
        {
            InitializeComponent();
            DataContext = this;
        }

        private static void OnThemeServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThemeSelector themeSelector)
            {
                if (e.OldValue is IThemeService oldService)
                {
                    oldService.ThemeChanged -= themeSelector.OnThemeChanged;
                }

                if (e.NewValue is IThemeService newService)
                {
                    newService.ThemeChanged += themeSelector.OnThemeChanged;
                    themeSelector.UpdateSelectedTheme();
                }
            }
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            UpdateSelectedTheme();
        }

        private void UpdateSelectedTheme()
        {
            if (ThemeService != null)
            {
                CurrentThemeIndex = ThemeService.CurrentTheme.Type switch
                {
                    ThemeType.Light => 0,
                    ThemeType.Dark => 1,
                    _ => 0
                };
            }
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Selection changed, ThemeService null: {ThemeService == null}");
                
                if (ThemeService != null && sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    var themeTag = selectedItem.Tag?.ToString();
                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: Selected theme tag: {themeTag}");
                    
                    if (Enum.TryParse<ThemeType>(themeTag, out var themeType))
                    {
                        System.Diagnostics.Debug.WriteLine($"ThemeSelector: Setting theme to: {themeType}");
                        ThemeService.SetTheme(themeType);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"ThemeSelector: Failed to parse theme type from: {themeTag}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: Selection change ignored - ThemeService: {ThemeService != null}, Sender: {sender?.GetType()}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error in selection changed: {ex.Message}");
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Toggle button clicked, ThemeService null: {ThemeService == null}");
                
                if (ThemeService != null)
                {
                    var currentTheme = ThemeService.CurrentTheme.Type;
                    var newTheme = currentTheme == ThemeType.Light ? ThemeType.Dark : ThemeType.Light;
                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: Toggling from {currentTheme} to {newTheme}");
                    ThemeService.SetTheme(newTheme);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ThemeSelector: Toggle ignored - ThemeService is null");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error in toggle: {ex.Message}");
            }
        }
    }
}