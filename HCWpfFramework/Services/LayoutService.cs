using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using HCWpfFramework.Interfaces;
using HCWpfFramework.Models;

namespace HCWpfFramework.Services
{
    /// <summary>
    /// Service for managing application layouts and providing default dockable windows
    /// Now integrates with WindowFactoryService for automatic window creation
    /// </summary>
    public class LayoutService : ILayoutService
    {
        private readonly Dictionary<DockingArea, Func<IEnumerable<DockableWindow>>> _windowFactories = new();
        private readonly IWindowFactoryService? _windowFactoryService;
        private LayoutType _currentLayout = LayoutType.ThreePane;

        public event EventHandler<LayoutChangedEventArgs>? LayoutChanged;

        public LayoutType CurrentLayout => _currentLayout;

        public IEnumerable<LayoutType> AvailableLayouts => Enum.GetValues<LayoutType>();

        public LayoutService(IWindowFactoryService? windowFactoryService = null)
        {
            _windowFactoryService = windowFactoryService;
            
            // Set initial layout to ThreePane
            _currentLayout = LayoutType.ThreePane;
            
            // Register default window factories
            RegisterDefaultWindowFactories();
        }

        public void SetLayout(LayoutType layoutType)
        {
            var oldLayout = _currentLayout;
            _currentLayout = layoutType;
            OnLayoutChanged(new LayoutChangedEventArgs(layoutType, oldLayout));
        }

        public ObservableCollection<DockableWindow> CreateDefaultWindows(DockingArea area)
        {
            var windows = new ObservableCollection<DockableWindow>();
            
            if (_windowFactories.TryGetValue(area, out var factory))
            {
                foreach (var window in factory())
                {
                    windows.Add(window);
                }
            }

            return windows;
        }

        public DefaultLayout CreateDefaultLayout()
        {
            var layout = new DefaultLayout
            {
                LayoutType = _currentLayout,
                LeftPanelWindows = CreateDefaultWindows(DockingArea.Left),
                RightPanelWindows = CreateDefaultWindows(DockingArea.Right),
                CenterPanelWindows = CreateDefaultWindows(DockingArea.Center),
                BottomPanelWindows = CreateDefaultWindows(DockingArea.Bottom),
                TopPanelWindows = CreateDefaultWindows(DockingArea.Top)
            };

            return layout;
        }
        
        /// <summary>
        /// Create layout using registered window factories for specific categories
        /// </summary>
        /// <param name="categories">Categories to include in the layout</param>
        /// <returns>Layout with windows from specified categories</returns>
        public DefaultLayout CreateLayoutWithCategories(params string[] categories)
        {
            var layout = new DefaultLayout { LayoutType = _currentLayout };
            
            if (_windowFactoryService != null)
            {
                foreach (var category in categories)
                {
                    var factories = _windowFactoryService.GetFactoriesByCategory(category);
                    foreach (var factory in factories)
                    {
                        var window = _windowFactoryService.CreateWindow(factory.WindowId);
                        if (window != null)
                        {
                            AddWindowToLayout(layout, window, factory.DefaultDockingArea);
                        }
                    }
                }
            }
            
            return layout;
        }
        
        private void AddWindowToLayout(DefaultLayout layout, DockableWindow window, DockingArea area)
        {
            switch (area)
            {
                case DockingArea.Left:
                    layout.LeftPanelWindows.Add(window);
                    break;
                case DockingArea.Right:
                    layout.RightPanelWindows.Add(window);
                    break;
                case DockingArea.Center:
                    layout.CenterPanelWindows.Add(window);
                    break;
                case DockingArea.Bottom:
                    layout.BottomPanelWindows.Add(window);
                    break;
                case DockingArea.Top:
                    layout.TopPanelWindows.Add(window);
                    break;
            }
        }

        public void RegisterWindowFactory(DockingArea area, Func<IEnumerable<DockableWindow>> factory)
        {
            _windowFactories[area] = factory;
        }

        public void SaveLayoutConfiguration()
        {
            try
            {
                var config = new { LayoutType = _currentLayout.ToString() };
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appFolder = Path.Combine(appDataPath, "HCWpfFramework");
                Directory.CreateDirectory(appFolder);
                
                var configFile = Path.Combine(appFolder, "layout-config.json");
                File.WriteAllText(configFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save layout configuration: {ex.Message}");
            }
        }

        public void LoadLayoutConfiguration()
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var configFile = Path.Combine(appDataPath, "HCWpfFramework", "layout-config.json");

                if (File.Exists(configFile))
                {
                    var json = File.ReadAllText(configFile);
                    var config = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    
                    if (config != null && config.TryGetValue("LayoutType", out var layoutTypeObj))
                    {
                        var layoutTypeElement = (JsonElement)layoutTypeObj;
                        if (Enum.TryParse<LayoutType>(layoutTypeElement.GetString(), out var layoutType))
                        {
                            SetLayout(layoutType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load layout configuration: {ex.Message}");
            }
        }

        public void ResetToDefault()
        {
            _windowFactories.Clear();
            RegisterDefaultWindowFactories();
            SetLayout(LayoutType.ThreePane);
        }

        private void RegisterDefaultWindowFactories()
        {
            // Left panel default windows
            RegisterWindowFactory(DockingArea.Left, () => new[]
            {
                new DockableWindow
                {
                    Id = "framework-explorer",
                    Title = "Explorer",
                    Description = "Project and solution structure explorer.\\n\\n" +
                                 "?? Framework Features:\\n" +
                                 "  ?? Services\\n" +
                                 "    ?? Theme Service\\n" +
                                 "    ?? Message Service\\n" +
                                 "    ?? Background Services\\n" +
                                 "  ?? Controls\\n" +
                                 "    ?? Docking Panels\\n" +
                                 "    ?? Theme Selector\\n" +
                                 "  ?? Models\\n" +
                                 "    ?? MVVM Components\\n\\n" +
                                 "Use this panel to navigate your application structure.",
                    CanClose = false,
                    CanFloat = true
                },
                new DockableWindow
                {
                    Id = "framework-toolbox",
                    Title = "Toolbox",
                    Description = "HCWpfFramework components and controls.\\n\\n" +
                                 "??? Layout Controls:\\n" +
                                 "  • DockingPanel\\n" +
                                 "  • Grid Splitters\\n" +
                                 "  • Themed Containers\\n\\n" +
                                 "??? Framework Components:\\n" +
                                 "  • ViewModelBase\\n" +
                                 "  • RelayCommand\\n" +
                                 "  • ThemeSelector\\n" +
                                 "  • Message Services\\n\\n" +
                                 "Drag and drop components into your application.",
                    CanClose = true,
                    CanFloat = true
                }
            });

            // Right panel default windows  
            RegisterWindowFactory(DockingArea.Right, () => new[]
            {
                new DockableWindow
                {
                    Id = "framework-properties",
                    Title = "Properties",
                    Description = "Property inspector for selected elements.\\n\\n" +
                                 "??? Framework Properties:\\n" +
                                 "  Theme: Dynamic theming support\\n" +
                                 "  Layout: Flexible docking system\\n" +
                                 "  Messaging: Inter-component communication\\n\\n" +
                                 "?? Control Properties:\\n" +
                                 "  • Appearance settings\\n" +
                                 "  • Behavior configuration\\n" +
                                 "  • Data binding options\\n\\n" +
                                 "Modify properties to customize your application.",
                    CanClose = true,
                    CanFloat = true
                }
            });

            // Center panel default windows
            RegisterWindowFactory(DockingArea.Center, () => new[]
            {
                new DockableWindow
                {
                    Id = "framework-welcome",
                    Title = "Welcome",
                    Description = "Welcome to HCWpfFramework! ??\\n\\n" +
                                 "? This framework provides:\\n" +
                                 "• ?? Dynamic theming (Light/Dark)\\n" +
                                 "• ?? Advanced docking system\\n" +
                                 "• ?? Thread-safe messaging\\n" +
                                 "• ??? MVVM architecture support\\n" +
                                 "• ?? Background services\\n" +
                                 "• ??? Dependency injection ready\\n\\n" +
                                 "?? Get started:\\n" +
                                 "1. Explore the framework components\\n" +
                                 "2. Try the theme switching (top menu)\\n" +
                                 "3. Test the docking functionality\\n" +
                                 "4. Send test messages between components\\n\\n" +
                                 "?? Check the documentation for advanced features!",
                    CanClose = true,
                    CanFloat = true
                }
            });

            // Bottom panel default windows
            RegisterWindowFactory(DockingArea.Bottom, () => new[]
            {
                new DockableWindow
                {
                    Id = "framework-output",
                    Title = "Output",
                    Description = "Framework and application output.\\n\\n" +
                                 "? Framework Status:\\n" +
                                 "• Theme Service: Initialized\\n" +
                                 "• Message Service: Active\\n" +
                                 "• Layout Service: Ready\\n" +
                                 "• Docking System: Operational\\n\\n" +
                                 "?? Recent Activity:\\n" +
                                 "• Application started successfully\\n" +
                                 "• Default layout applied\\n" +
                                 "• Theme loaded from preferences\\n" +
                                 "• All framework services registered\\n\\n" +
                                 "?? Monitor this panel for system messages and logs.",
                    CanClose = true,
                    CanFloat = true
                },
                new DockableWindow
                {
                    Id = "framework-messages",
                    Title = "Messages",
                    Description = "Framework messaging system activity.\\n\\n" +
                                 "?? Message Types:\\n" +
                                 "• Information: General notifications\\n" +
                                 "• Warning: Non-critical alerts\\n" +
                                 "• Error: Critical issues\\n" +
                                 "• Debug: Development information\\n\\n" +
                                 "?? Recent Messages:\\n" +
                                 "• Framework initialized successfully\\n" +
                                 "• Theme service ready\\n" +
                                 "• Layout service active\\n\\n" +
                                 "Use the messaging system to communicate between components.",
                    CanClose = true,
                    CanFloat = true
                }
            });
        }

        private void OnLayoutChanged(LayoutChangedEventArgs e)
        {
            LayoutChanged?.Invoke(this, e);
        }
    }
}