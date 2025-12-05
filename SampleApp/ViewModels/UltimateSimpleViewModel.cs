using System;
using System.Windows;
using System.Windows.Controls;
using HCWpfFramework.Interfaces;
using HCWpfFramework.ViewModels;
using HCWpfFramework.Models;
using HCWpfFramework.Commands;
using SampleApp.WindowFactories;

namespace SampleApp.ViewModels
{
    /// <summary>
    /// Ultimate demonstration of framework simplicity
    /// This ViewModel focuses ONLY on sample-specific functionality
    /// All infrastructure is handled by the framework
    /// </summary>
    public class UltimateSimpleViewModel : MainViewModelBase
    {
        private readonly IWindowFactoryService _windowFactoryService;

        public UltimateSimpleViewModel(
            IMessageService messageService, 
            IThemeService themeService, 
            ILayoutService layoutService,
            IWindowFactoryService windowFactoryService) 
            : base(messageService, themeService, layoutService)
        {
            _windowFactoryService = windowFactoryService ?? throw new ArgumentNullException(nameof(windowFactoryService));
            
            Title = "HCWpfFramework - Ultimate Simplicity Demo";
            StatusMessage = "Framework handles everything - you just create windows!";
            
            // Register sample window factories
            RegisterSampleWindowFactories();
            
            // Set custom menu for window creation
            SetWindowCreationMenu();
            
            // Set initial layout (this will be overridden by framework initialization)
            CurrentLayoutType = LayoutType.ThreePane;
        }

        private void RegisterSampleWindowFactories()
        {
            if (_windowFactoryService == null)
            {
                StatusMessage = "Cannot register window factories - service is null";
                return;
            }
            
            try
            {
                // Register all sample window factories
                _windowFactoryService.RegisterFactory(new WelcomeWindowFactory());
                _windowFactoryService.RegisterFactory(new DataExplorerFactory());
                _windowFactoryService.RegisterFactory(new ToolsWindowFactory());
                _windowFactoryService.RegisterFactory(new LogViewerFactory());
                _windowFactoryService.RegisterFactory(new DynamicContentFactory());
                
                StatusMessage = "Sample window factories registered successfully";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registering window factories: {ex.Message}");
                StatusMessage = $"Error registering window factories: {ex.Message}";
            }
        }

        protected override void CustomizeLayout()
        {
            // Run diagnostics to help debug any issues
            DiagnoseWindowFactoryService();
            
            // Framework automatically creates windows from registered factories
            // We just specify which categories to include
            CreateWindowsFromCategories("Sample", "Data", "Tools", "Debug");
        }

        private void CreateWindowsFromCategories(params string[] categories)
        {
            if (_windowFactoryService == null)
            {
                StatusMessage = "Window factory service is not available";
                return;
            }
            
            foreach (var category in categories)
            {
                try
                {
                    var factories = _windowFactoryService.GetFactoriesByCategory(category);
                    if (factories == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"No factories found for category: {category}");
                        continue;
                    }
                    
                    foreach (var factory in factories)
                    {
                        if (factory == null) continue;
                        
                        var window = _windowFactoryService.CreateWindow(factory.WindowId);
                        if (window != null)
                        {
                            AddWindowToCorrectPanel(window, factory.DefaultDockingArea);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error creating windows for category {category}: {ex.Message}");
                    StatusMessage = $"Error creating windows for category {category}";
                }
            }
        }

        private void AddWindowToCorrectPanel(DockableWindow window, DockingArea area)
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
            }
        }

        private void SetWindowCreationMenu()
        {
            try
            {
                // Create menu for adding new windows dynamically
                var windowMenu = new MenuItem { Header = "Windows" };
                
                // Apply style safely
                if (System.Windows.Application.Current?.FindResource("ThemedMenuItemStyle") is Style menuStyle)
                {
                    windowMenu.Style = menuStyle;
                }

                // Add menu items for each registered factory
                foreach (var factory in _windowFactoryService.GetAllFactories())
                {
                    var menuItem = new MenuItem
                    {
                        Header = $"Add {factory.DisplayName}",
                        Command = new RelayCommand(() => CreateWindowFromFactory(factory))
                    };
                    
                    // Apply submenu style safely
                    if (System.Windows.Application.Current?.FindResource("ThemedSubmenuItemStyle") is Style submenuStyle)
                    {
                        menuItem.Style = submenuStyle;
                    }
                    
                    windowMenu.Items.Add(menuItem);
                }

                CustomMenuContent = windowMenu;
            }
            catch (Exception ex)
            {
                // If menu creation fails, just log and continue without custom menu
                System.Diagnostics.Debug.WriteLine($"Failed to create window menu: {ex.Message}");
                StatusMessage = "Framework loaded successfully (menu creation skipped)";
            }
        }

        private void CreateWindowFromFactory(IWindowFactory factory)
        {
            if (_windowFactoryService == null)
            {
                StatusMessage = "Window factory service is null";
                return;
            }
            
            var window = _windowFactoryService.CreateWindow(factory.WindowId);
            if (window != null)
            {
                AddWindowToCorrectPanel(window, factory.DefaultDockingArea);
                StatusMessage = $"Created {factory.DisplayName} window";
                MessageService.SendMessage(MessageType.Information, ViewModelId, 
                    $"Window '{factory.DisplayName}' created from factory '{factory.WindowId}'");
            }
        }
        
        /// <summary>
        /// Diagnostic method to check the state of the window factory service
        /// </summary>
        public void DiagnoseWindowFactoryService()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== Window Factory Service Diagnostics ===");
                System.Diagnostics.Debug.WriteLine($"Service is null: {_windowFactoryService == null}");
                
                if (_windowFactoryService != null)
                {
                    var allFactories = _windowFactoryService.GetAllFactories()?.ToList();
                    System.Diagnostics.Debug.WriteLine($"Total factories registered: {allFactories?.Count ?? 0}");
                    
                    if (allFactories != null)
                    {
                        foreach (var factory in allFactories)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - {factory?.WindowId}: {factory?.DisplayName} ({factory?.Category})");
                        }
                    }
                    
                    var categories = new[] { "Sample", "Data", "Tools", "Debug" };
                    foreach (var category in categories)
                    {
                        var categoryFactories = _windowFactoryService.GetFactoriesByCategory(category)?.ToList();
                        System.Diagnostics.Debug.WriteLine($"Category '{category}': {categoryFactories?.Count ?? 0} factories");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in diagnostics: {ex.Message}");
            }
        }
    }
}