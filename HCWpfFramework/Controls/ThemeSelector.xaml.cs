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

        private System.Windows.Threading.DispatcherTimer? _themeMonitorTimer;
        private ThemeType _lastKnownTheme = ThemeType.Light;
        
        public ThemeSelector()
        {
            InitializeComponent();
            DataContext = this;
            
            // Try to find ThemeService automatically if not provided
            Loaded += ThemeSelector_Loaded;
            Unloaded += ThemeSelector_Unloaded;
            
            // Start monitoring application resources for theme changes
            StartThemeMonitoring();
        }
        
        private void ThemeSelector_Unloaded(object sender, RoutedEventArgs e)
        {
            StopThemeMonitoring();
        }
        
        private void StartThemeMonitoring()
        {
            try
            {
                _themeMonitorTimer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(500) // Check every 500ms
                };
                _themeMonitorTimer.Tick += ThemeMonitorTimer_Tick;
                _themeMonitorTimer.Start();
                
                System.Diagnostics.Debug.WriteLine("ThemeSelector: Started theme monitoring");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error starting theme monitoring: {ex.Message}");
            }
        }
        
        private void StopThemeMonitoring()
        {
            try
            {
                _themeMonitorTimer?.Stop();
                _themeMonitorTimer = null;
                System.Diagnostics.Debug.WriteLine("ThemeSelector: Stopped theme monitoring");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error stopping theme monitoring: {ex.Message}");
            }
        }
        
        private void ThemeMonitorTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                // Detect current theme by checking application resources
                var currentTheme = DetectCurrentThemeFromResources();
                
                if (currentTheme != _lastKnownTheme)
                {
                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: Theme change detected - {_lastKnownTheme} -> {currentTheme}");
                    _lastKnownTheme = currentTheme;
                    
                    // Update the ComboBox selection to match
                    var newIndex = currentTheme switch
                    {
                        ThemeType.Light => 0,
                        ThemeType.Dark => 1,
                        _ => 0
                    };
                    
                    if (ThemeComboBox.SelectedIndex != newIndex)
                    {
                        _isUpdatingSelection = true;
                        ThemeComboBox.SelectedIndex = newIndex;
                        CurrentThemeIndex = newIndex;
                        _isUpdatingSelection = false;
                        
                        System.Diagnostics.Debug.WriteLine($"ThemeSelector: Updated ComboBox selection to {newIndex} for theme {currentTheme}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error in theme monitor: {ex.Message}");
            }
        }
        
        private ThemeType DetectCurrentThemeFromResources()
        {
            try
            {
                var app = Application.Current;
                if (app?.Resources != null)
                {
                    // Check the primary background brush to determine theme
                    if (app.Resources["PrimaryBackgroundBrush"] is System.Windows.Media.SolidColorBrush brush)
                    {
                        var color = brush.Color;
                        
                        // Light theme has light background (high luminance)
                        // Dark theme has dark background (low luminance)
                        var luminance = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;
                        
                        return luminance > 127 ? ThemeType.Light : ThemeType.Dark;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error detecting theme from resources: {ex.Message}");
            }
            
            return ThemeType.Light; // Default fallback
        }
        
        private void ThemeSelector_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ThemeSelector: Loaded event fired");
                
                // If no ThemeService is set, try to find it automatically
                if (ThemeService == null)
                {
                    System.Diagnostics.Debug.WriteLine("ThemeSelector: ThemeService is null, attempting auto-discovery");
                    
                    // Method 1: Try to get from ServiceProvider in Application
                    var app = Application.Current;
                    if (app?.Properties["ServiceProvider"] is IServiceProvider serviceProvider)
                    {
                        var themeService = serviceProvider.GetService(typeof(IThemeService)) as IThemeService;
                        if (themeService != null)
                        {
                            System.Diagnostics.Debug.WriteLine("ThemeSelector: Found ThemeService via ServiceProvider");
                            ThemeService = themeService;
                            return;
                        }
                    }
                    
                    // Method 2: Try to get from DataContext chain
                    var element = this as FrameworkElement;
                    while (element != null)
                    {
                        if (element.DataContext is HCWpfFramework.ViewModels.MainViewModelBase viewModel)
                        {
                            var themeService = viewModel.GetThemeService;
                            if (themeService != null)
                            {
                                System.Diagnostics.Debug.WriteLine("ThemeSelector: Found ThemeService via DataContext chain");
                                ThemeService = themeService;
                                return;
                            }
                        }
                        element = element.Parent as FrameworkElement ?? 
                                 System.Windows.Media.VisualTreeHelper.GetParent(element) as FrameworkElement;
                    }
                    
                    System.Diagnostics.Debug.WriteLine("ThemeSelector: Could not auto-discover ThemeService");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: ThemeService already set: {ThemeService.CurrentTheme.Type}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error in Loaded event: {ex.Message}");
            }
        }

        private static void OnThemeServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThemeSelector themeSelector)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: OnThemeServiceChanged - Old: {e.OldValue != null}, New: {e.NewValue != null}");
                
                if (e.OldValue is IThemeService oldService)
                {
                    oldService.ThemeChanged -= themeSelector.OnThemeChanged;
                    System.Diagnostics.Debug.WriteLine("ThemeSelector: Unsubscribed from old ThemeService");
                }

                if (e.NewValue is IThemeService newService)
                {
                    newService.ThemeChanged += themeSelector.OnThemeChanged;
                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: Subscribed to new ThemeService - Current theme: {newService.CurrentTheme.Type}");
                    
                    // Update immediately and then again after a short delay to ensure proper synchronization
                    themeSelector.UpdateSelectedTheme();
                    
                    themeSelector.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        themeSelector.UpdateSelectedTheme();
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ThemeSelector: No ThemeService provided");
                }
            }
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            UpdateSelectedTheme();
        }

        private void UpdateSelectedTheme()
        {
            try
            {
                if (ThemeService != null)
                {
                    var newIndex = ThemeService.CurrentTheme.Type switch
                    {
                        ThemeType.Light => 0,
                        ThemeType.Dark => 1,
                        _ => 0
                    };
                    
                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: UpdateSelectedTheme - Current: {ThemeService.CurrentTheme.Type}, NewIndex: {newIndex}, CurrentIndex: {CurrentThemeIndex}");
                    
                    // Only update if different to avoid recursive calls
                    if (CurrentThemeIndex != newIndex)
                    {
                        _isUpdatingSelection = true;
                        try
                        {
                            CurrentThemeIndex = newIndex;
                            
                            // Also update ComboBox directly to ensure synchronization
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if (ThemeComboBox.SelectedIndex != newIndex)
                                {
                                    ThemeComboBox.SelectedIndex = newIndex;
                                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: ComboBox SelectedIndex updated to {newIndex}");
                                }
                                _isUpdatingSelection = false;
                            }));
                        }
                        catch
                        {
                            _isUpdatingSelection = false;
                            throw;
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ThemeSelector: UpdateSelectedTheme - ThemeService is null");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error in UpdateSelectedTheme: {ex.Message}");
                _isUpdatingSelection = false;
            }
        }

        private bool _isUpdatingSelection = false;
        
        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Prevent recursive updates
                if (_isUpdatingSelection)
                {
                    System.Diagnostics.Debug.WriteLine("ThemeSelector: Selection change ignored - updating in progress");
                    return;
                }
                
                var selectedIndex = ThemeComboBox.SelectedIndex;
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Selection changed to index {selectedIndex}");
                
                // Determine target theme from selection
                var targetTheme = selectedIndex switch
                {
                    0 => ThemeType.Light,
                    1 => ThemeType.Dark,
                    _ => ThemeType.Light
                };
                
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Target theme: {targetTheme}");
                
                // Try multiple methods to set the theme
                bool themeChanged = false;
                
                // Method 1: Use bound ThemeService
                if (ThemeService != null)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("ThemeSelector: Attempting theme change via bound ThemeService");
                        ThemeService.SetTheme(targetTheme);
                        themeChanged = true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error with bound ThemeService: {ex.Message}");
                    }
                }
                
                // Method 2: Try ServiceProvider
                if (!themeChanged)
                {
                    try
                    {
                        var app = Application.Current;
                        if (app?.Properties["ServiceProvider"] is IServiceProvider serviceProvider)
                        {
                            var themeService = serviceProvider.GetService(typeof(IThemeService)) as IThemeService;
                            if (themeService != null)
                            {
                                System.Diagnostics.Debug.WriteLine("ThemeSelector: Attempting theme change via ServiceProvider");
                                themeService.SetTheme(targetTheme);
                                themeChanged = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error with ServiceProvider method: {ex.Message}");
                    }
                }
                
                // Method 3: Direct application resource manipulation as fallback
                if (!themeChanged)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("ThemeSelector: Attempting direct resource manipulation");
                        ApplyThemeDirectly(targetTheme);
                        themeChanged = true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error with direct method: {ex.Message}");
                    }
                }
                
                if (themeChanged)
                {
                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: Successfully changed theme to {targetTheme}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ThemeSelector: Failed to change theme using all methods");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error in selection changed: {ex.Message}");
            }
        }
        
        private void ApplyThemeDirectly(ThemeType themeType)
        {
            var app = Application.Current;
            if (app?.Resources == null) return;
            
            System.Diagnostics.Debug.WriteLine($"ThemeSelector: Applying {themeType} theme directly to resources");
            
            var resources = GetThemeResources(themeType);
            foreach (var resource in resources)
            {
                try
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = converter.ConvertFromString(resource.Value);
                    if (brush != null)
                    {
                        app.Resources[resource.Key] = brush;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error applying resource {resource.Key}: {ex.Message}");
                }
            }
            
            // Force UI refresh
            foreach (System.Windows.Window window in app.Windows)
            {
                try
                {
                    window.InvalidateVisual();
                    window.UpdateLayout();
                    
                    // Try to update the ViewModel's CurrentThemeName
                    if (window.DataContext is HCWpfFramework.ViewModels.MainViewModelBase viewModel)
                    {
                        viewModel.CurrentThemeName = themeType == ThemeType.Dark ? "Dark" : "Light";
                        System.Diagnostics.Debug.WriteLine($"ThemeSelector: Updated ViewModel CurrentThemeName to {viewModel.CurrentThemeName}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error refreshing window: {ex.Message}");
                }
            }
        }
        
        private Dictionary<string, string> GetThemeResources(ThemeType themeType)
        {
            return themeType == ThemeType.Dark ? GetDarkThemeResources() : GetLightThemeResources();
        }
        
        private Dictionary<string, string> GetLightThemeResources()
        {
            return new Dictionary<string, string>
            {
                ["PrimaryBackgroundBrush"] = "#FFFFFF",
                ["SecondaryBackgroundBrush"] = "#F5F5F5",
                ["PrimaryTextBrush"] = "#212529",
                ["SecondaryTextBrush"] = "#6C757D",
                ["MenuBackgroundBrush"] = "#FFFFFF",
                ["MenuTextBrush"] = "#212529",
                ["StatusBarBackgroundBrush"] = "#F8F9FA",
                ["ButtonBackgroundBrush"] = "#F8F9FA",
                ["BorderBrush"] = "#DEE2E6"
            };
        }
        
        private Dictionary<string, string> GetDarkThemeResources()
        {
            return new Dictionary<string, string>
            {
                ["PrimaryBackgroundBrush"] = "#1E1E1E",
                ["SecondaryBackgroundBrush"] = "#252526",
                ["PrimaryTextBrush"] = "#FFFFFF",
                ["SecondaryTextBrush"] = "#CCCCCC",
                ["MenuBackgroundBrush"] = "#2D2D30",
                ["MenuTextBrush"] = "#FFFFFF",
                ["StatusBarBackgroundBrush"] = "#007ACC",
                ["ButtonBackgroundBrush"] = "#333333",
                ["BorderBrush"] = "#3C3C3C"
            };
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
                    
                    // Try to get ThemeService from application resources as fallback
                    TryManualThemeChange();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error in toggle: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Manual method to test theme changes when binding fails
        /// </summary>
        private void TryManualThemeChange()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ThemeSelector: Attempting manual theme change");
                
                // Try to get service provider from application
                var app = Application.Current;
                var serviceProvider = app?.Properties["ServiceProvider"] as IServiceProvider;
                
                if (serviceProvider != null)
                {
                    var themeService = serviceProvider.GetService(typeof(IThemeService)) as IThemeService;
                    if (themeService != null)
                    {
                        System.Diagnostics.Debug.WriteLine("ThemeSelector: Found ThemeService via ServiceProvider");
                        ThemeService = themeService;
                        
                        // Now try the toggle again
                        var currentTheme = themeService.CurrentTheme.Type;
                        var newTheme = currentTheme == ThemeType.Light ? ThemeType.Dark : ThemeType.Light;
                        themeService.SetTheme(newTheme);
                        return;
                    }
                }
                
                System.Diagnostics.Debug.WriteLine("ThemeSelector: Could not find ThemeService via any method");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: Error in manual theme change: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Diagnostic method to check ThemeSelector state
        /// </summary>
        public void DiagnoseThemeSelector()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== ThemeSelector Diagnostics ===");
                System.Diagnostics.Debug.WriteLine($"ThemeService is null: {ThemeService == null}");
                System.Diagnostics.Debug.WriteLine($"CurrentThemeIndex: {CurrentThemeIndex}");
                System.Diagnostics.Debug.WriteLine($"IsUpdatingSelection: {_isUpdatingSelection}");
                System.Diagnostics.Debug.WriteLine($"DataContext: {DataContext?.GetType()?.Name}");
                
                if (ThemeService != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Current Theme: {ThemeService.CurrentTheme.Name}");
                    System.Diagnostics.Debug.WriteLine($"Current Theme Type: {ThemeService.CurrentTheme.Type}");
                    System.Diagnostics.Debug.WriteLine($"Available Themes: {string.Join(", ", ThemeService.AvailableThemes.Select(t => t.Name))}");
                }
                
                // Check ComboBox state
                System.Diagnostics.Debug.WriteLine($"ComboBox SelectedIndex: {ThemeComboBox.SelectedIndex}");
                System.Diagnostics.Debug.WriteLine($"ComboBox Items Count: {ThemeComboBox.Items.Count}");
                
                for (int i = 0; i < ThemeComboBox.Items.Count; i++)
                {
                    if (ThemeComboBox.Items[i] is ComboBoxItem item)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Item {i}: {item.Content}, Tag: {item.Tag}");
                    }
                }
                
                // Check synchronization
                if (ThemeService != null)
                {
                    var expectedIndex = ThemeService.CurrentTheme.Type switch
                    {
                        ThemeType.Light => 0,
                        ThemeType.Dark => 1,
                        _ => 0
                    };
                    var isInSync = (CurrentThemeIndex == expectedIndex && ThemeComboBox.SelectedIndex == expectedIndex);
                    System.Diagnostics.Debug.WriteLine($"Synchronization: Expected={expectedIndex}, CurrentThemeIndex={CurrentThemeIndex}, ComboBox={ThemeComboBox.SelectedIndex}, InSync={isInSync}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector diagnostics failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Manually synchronize the theme selector with the current theme
        /// </summary>
        public void ManualSync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ThemeSelector: ManualSync called");
                _isUpdatingSelection = false; // Reset the flag
                UpdateSelectedTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeSelector: ManualSync failed: {ex.Message}");
            }
        }
    }
}