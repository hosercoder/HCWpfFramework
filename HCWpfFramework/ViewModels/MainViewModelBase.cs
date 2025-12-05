using System.Collections.ObjectModel;
using System.Windows.Input;
using HCWpfFramework.Commands;
using HCWpfFramework.Interfaces;
using HCWpfFramework.Models;

namespace HCWpfFramework.ViewModels
{
    /// <summary>
    /// Base ViewModel for main application windows that provides framework layout integration
    /// </summary>
    public abstract class MainViewModelBase : ViewModelBase
    {
        protected readonly IMessageService MessageService;
        protected readonly IThemeService ThemeService;
        protected readonly ILayoutService LayoutService;
        protected readonly string ViewModelId;

        private string _title = "HCWpfFramework Application";
        private string _statusMessage = "Ready";
        private LayoutType _currentLayoutType;

        public MainViewModelBase(
            IMessageService messageService, 
            IThemeService themeService, 
            ILayoutService layoutService)
        {
            MessageService = messageService;
            ThemeService = themeService;
            LayoutService = layoutService;
            ViewModelId = $"{GetType().Name}_{Environment.TickCount}";

            // Initialize layout
            InitializeLayout();

            // Initialize commands
            InitializeFrameworkCommands();

            // Subscribe to services
            SubscribeToServices();
        }

        #region Properties

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public LayoutType CurrentLayoutType
        {
            get => _currentLayoutType;
            set => SetProperty(ref _currentLayoutType, value);
        }

        public IThemeService GetThemeService => ThemeService;
        public IMessageService GetMessageService => MessageService;
        public ILayoutService GetLayoutService => LayoutService;
        
        private object? _customMenuContent;
        public object? CustomMenuContent
        {
            get => _customMenuContent;
            set => SetProperty(ref _customMenuContent, value);
        }
        
        private object? _customStatusContent;
        public object? CustomStatusContent
        {
            get => _customStatusContent;
            set => SetProperty(ref _customStatusContent, value);
        }

        public ObservableCollection<DockableWindow> LeftPanelWindows { get; protected set; } = new();
        public ObservableCollection<DockableWindow> RightPanelWindows { get; protected set; } = new();
        public ObservableCollection<DockableWindow> CenterPanelWindows { get; protected set; } = new();
        public ObservableCollection<DockableWindow> BottomPanelWindows { get; protected set; } = new();
        public ObservableCollection<DockableWindow> TopPanelWindows { get; protected set; } = new();

        #endregion

        #region Framework Commands

        public ICommand NewCommand { get; private set; } = null!;
        public ICommand OpenCommand { get; private set; } = null!;
        public ICommand ChangeLayoutCommand { get; private set; } = null!;
        public ICommand ResetLayoutCommand { get; private set; } = null!;
        public ICommand SaveLayoutCommand { get; private set; } = null!;
        public ICommand LoadLayoutCommand { get; private set; } = null!;
        public ICommand ExitCommand { get; private set; } = null!;
        public ICommand TestLayoutCommand { get; private set; } = null!;

        #endregion

        #region Virtual Methods for Override

        /// <summary>
        /// Override to customize the default layout with application-specific windows
        /// </summary>
        protected virtual void CustomizeLayout()
        {
            // Default implementation - can be overridden by derived classes
        }

        /// <summary>
        /// Override to add application-specific commands
        /// </summary>
        protected virtual void InitializeCustomCommands()
        {
            // Default implementation - can be overridden by derived classes
        }

        /// <summary>
        /// Override to handle custom message types
        /// </summary>
        protected virtual void HandleCustomMessage(IMessage message)
        {
            // Default implementation - can be overridden by derived classes
        }

        /// <summary>
        /// Override to handle layout changes
        /// </summary>
        protected virtual void OnLayoutChanged(LayoutChangedEventArgs e)
        {
            StatusMessage = $"Layout changed to {e.NewLayout}";
        }

        /// <summary>
        /// Override to handle theme changes
        /// </summary>
        protected virtual void OnThemeChanged(ThemeChangedEventArgs e)
        {
            StatusMessage = $"Theme changed to {e.NewTheme.Name}";
        }

        #endregion

        #region Window Management

        public void OnWindowFloatRequested(object sender, DockableWindow window)
        {
            RemoveWindowFromAllPanels(window);
            StatusMessage = $"Float requested for {window.Title}";
            MessageService.SendMessage(MessageType.Information, ViewModelId, $"Window '{window.Title}' float requested");
        }

        public void OnWindowCloseRequested(object sender, DockableWindow window)
        {
            RemoveWindowFromAllPanels(window);
            StatusMessage = $"Closed {window.Title}";
            MessageService.SendMessage(MessageType.Information, ViewModelId, $"Window '{window.Title}' closed");
        }

        protected void RemoveWindowFromAllPanels(DockableWindow window)
        {
            LeftPanelWindows.Remove(window);
            RightPanelWindows.Remove(window);
            CenterPanelWindows.Remove(window);
            BottomPanelWindows.Remove(window);
            TopPanelWindows.Remove(window);
        }

        #endregion

        #region Private Methods

        private void InitializeLayout()
        {
            System.Diagnostics.Debug.WriteLine("MainViewModelBase: InitializeLayout starting...");
            
            // Load saved layout configuration
            LayoutService.LoadLayoutConfiguration();
            
            // Create default layout
            var defaultLayout = LayoutService.CreateDefaultLayout();
            System.Diagnostics.Debug.WriteLine($"MainViewModelBase: Default layout type: {defaultLayout.LayoutType}");
            
            // Set current layout type FIRST for UI binding
            CurrentLayoutType = defaultLayout.LayoutType;
            System.Diagnostics.Debug.WriteLine($"MainViewModelBase: CurrentLayoutType set to: {CurrentLayoutType}");
            
            // Apply layout to collections
            ApplyLayout(defaultLayout);
            
            // Allow customization
            CustomizeLayout();
            
            StatusMessage = $"Framework layout initialized ({CurrentLayoutType})";
            System.Diagnostics.Debug.WriteLine($"MainViewModelBase: InitializeLayout completed with: {CurrentLayoutType}");
        }

        private void ApplyLayout(DefaultLayout layout)
        {
            // Clear existing collections
            LeftPanelWindows.Clear();
            RightPanelWindows.Clear();
            CenterPanelWindows.Clear();
            BottomPanelWindows.Clear();
            TopPanelWindows.Clear();

            // Apply new layout
            foreach (var window in layout.LeftPanelWindows)
                LeftPanelWindows.Add(window);
            
            foreach (var window in layout.RightPanelWindows)
                RightPanelWindows.Add(window);
            
            foreach (var window in layout.CenterPanelWindows)
                CenterPanelWindows.Add(window);
            
            foreach (var window in layout.BottomPanelWindows)
                BottomPanelWindows.Add(window);
            
            foreach (var window in layout.TopPanelWindows)
                TopPanelWindows.Add(window);
        }

        private void InitializeFrameworkCommands()
        {
            NewCommand = new RelayCommand(ExecuteNew);
            OpenCommand = new RelayCommand(ExecuteOpen);
            ChangeLayoutCommand = new RelayCommand<string>(ExecuteChangeLayoutFromString);
            ResetLayoutCommand = new RelayCommand(ExecuteResetLayout);
            SaveLayoutCommand = new RelayCommand(ExecuteSaveLayout);
            LoadLayoutCommand = new RelayCommand(ExecuteLoadLayout);
            ExitCommand = new RelayCommand(ExecuteExit);
            TestLayoutCommand = new RelayCommand(TestFrameworkFunctionality);

            // Initialize custom commands
            InitializeCustomCommands();
        }

        private void SubscribeToServices()
        {
            MessageService.Subscribe(ViewModelId, HandleMessage);
            ThemeService.ThemeChanged += OnFrameworkThemeChanged;
            LayoutService.LayoutChanged += OnFrameworkLayoutChanged;
        }

        private void ExecuteChangeLayoutFromString(string? layoutTypeString)
        {
            System.Diagnostics.Debug.WriteLine($"MainViewModelBase: ExecuteChangeLayoutFromString called with: {layoutTypeString}");
            
            if (string.IsNullOrEmpty(layoutTypeString))
            {
                System.Diagnostics.Debug.WriteLine($"MainViewModelBase: Layout string is null or empty");
                return;
            }
                
            if (Enum.TryParse<LayoutType>(layoutTypeString, out var layoutType))
            {
                System.Diagnostics.Debug.WriteLine($"MainViewModelBase: Parsed layout type: {layoutType}");
                ExecuteChangeLayout(layoutType);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"MainViewModelBase: Failed to parse layout type from: {layoutTypeString}");
            }
        }
        
        private void ExecuteChangeLayout(LayoutType layoutType)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"MainViewModelBase: ExecuteChangeLayout called with: {layoutType}");
                System.Diagnostics.Debug.WriteLine($"MainViewModelBase: Current layout: {CurrentLayoutType}");
                
                // Update the CurrentLayoutType immediately for UI binding
                CurrentLayoutType = layoutType;
                
                // Update the layout service
                LayoutService.SetLayout(layoutType);
                
                System.Diagnostics.Debug.WriteLine($"MainViewModelBase: Layout set successfully to {layoutType}");
                StatusMessage = $"Layout changed to {layoutType}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MainViewModelBase: Error setting layout: {ex.Message}");
                StatusMessage = $"Error changing layout: {ex.Message}";
            }
        }

        private void ExecuteResetLayout()
        {
            LayoutService.ResetToDefault();
            var defaultLayout = LayoutService.CreateDefaultLayout();
            ApplyLayout(defaultLayout);
            CurrentLayoutType = defaultLayout.LayoutType;
            
            StatusMessage = "Layout reset to default";
            MessageService.SendMessage(MessageType.Information, ViewModelId, "Layout reset to default");
        }

        private void ExecuteSaveLayout()
        {
            LayoutService.SaveLayoutConfiguration();
            StatusMessage = "Layout configuration saved";
            MessageService.SendMessage(MessageType.Information, ViewModelId, "Layout configuration saved");
        }

        private void ExecuteLoadLayout()
        {
            LayoutService.LoadLayoutConfiguration();
            var layout = LayoutService.CreateDefaultLayout();
            ApplyLayout(layout);
            CurrentLayoutType = layout.LayoutType;
            
            StatusMessage = "Layout configuration loaded";
            MessageService.SendMessage(MessageType.Information, ViewModelId, "Layout configuration loaded");
        }
        
        private void ExecuteExit()
        {
            System.Windows.Application.Current.Shutdown();
        }
        
        /// <summary>
        /// Virtual method for New command - can be overridden by derived classes
        /// </summary>
        protected virtual void ExecuteNew()
        {
            System.Diagnostics.Debug.WriteLine("MainViewModelBase: New command executed");
            StatusMessage = "New command executed";
            MessageService.SendMessage(MessageType.Information, ViewModelId, "New command executed");
        }
        
        /// <summary>
        /// Virtual method for Open command - can be overridden by derived classes
        /// </summary>
        protected virtual void ExecuteOpen()
        {
            StatusMessage = "Open command executed";
            MessageService.SendMessage(MessageType.Information, ViewModelId, "Open command executed");
        }

        private void HandleMessage(IMessage message)
        {
            StatusMessage = $"Received: {message.MessageType} from {message.SenderId} at {message.Timestamp:HH:mm:ss}";
            
            // Allow derived classes to handle custom messages
            HandleCustomMessage(message);
        }

        private void OnFrameworkThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            OnThemeChanged(e);
        }

        private void OnFrameworkLayoutChanged(object? sender, LayoutChangedEventArgs e)
        {
            var layout = LayoutService.CreateDefaultLayout();
            ApplyLayout(layout);
            CurrentLayoutType = e.NewLayout;
            OnLayoutChanged(e);
        }

        /// <summary>
        /// Diagnostic method to test framework functionality
        /// </summary>
        public void TestFrameworkFunctionality()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== Framework Functionality Test ===");
                System.Diagnostics.Debug.WriteLine($"ViewModel Type: {GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Current Layout: {CurrentLayoutType}");
                System.Diagnostics.Debug.WriteLine($"Theme Service Available: {ThemeService != null}");
                System.Diagnostics.Debug.WriteLine($"Layout Service Available: {LayoutService != null}");
                System.Diagnostics.Debug.WriteLine($"Message Service Available: {MessageService != null}");
                
                if (ThemeService != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Current Theme: {ThemeService.CurrentTheme.Name}");
                    System.Diagnostics.Debug.WriteLine($"Current Theme Type: {ThemeService.CurrentTheme.Type}");
                }
                
                // Test layout change
                System.Diagnostics.Debug.WriteLine($"Current CurrentLayoutType value: {CurrentLayoutType}");
                System.Diagnostics.Debug.WriteLine($"Current LayoutService.CurrentLayout: {LayoutService.CurrentLayout}");
                
                System.Diagnostics.Debug.WriteLine("Testing layout change to Single...");
                ExecuteChangeLayout(LayoutType.Single);
                System.Diagnostics.Debug.WriteLine($"After Single - CurrentLayoutType: {CurrentLayoutType}");
                
                System.Threading.Thread.Sleep(100); // Give UI time to update
                
                System.Diagnostics.Debug.WriteLine("Testing layout change to TopBottom...");
                ExecuteChangeLayout(LayoutType.TopBottom);
                System.Diagnostics.Debug.WriteLine($"After TopBottom - CurrentLayoutType: {CurrentLayoutType}");
                
                System.Threading.Thread.Sleep(100);
                
                System.Diagnostics.Debug.WriteLine("Testing layout change back to ThreePane...");
                ExecuteChangeLayout(LayoutType.ThreePane);
                System.Diagnostics.Debug.WriteLine($"After ThreePane - CurrentLayoutType: {CurrentLayoutType}");
                
                System.Threading.Thread.Sleep(100);
                
                System.Diagnostics.Debug.WriteLine("Testing layout change to FourPane...");
                ExecuteChangeLayout(LayoutType.FourPane);
                System.Diagnostics.Debug.WriteLine($"After FourPane - CurrentLayoutType: {CurrentLayoutType}");
                
                // Test theme change if available
                if (ThemeService != null)
                {
                    System.Diagnostics.Debug.WriteLine("Testing theme toggle...");
                    var currentTheme = ThemeService.CurrentTheme.Type;
                    var newTheme = currentTheme == ThemeType.Light ? ThemeType.Dark : ThemeType.Light;
                    ThemeService.SetTheme(newTheme);
                    System.Diagnostics.Debug.WriteLine($"Theme changed to: {ThemeService.CurrentTheme.Type}");
                }
                
                StatusMessage = "Framework functionality test completed - check debug output";
                System.Diagnostics.Debug.WriteLine("=== Framework Test Completed ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Framework test failed: {ex.Message}");
                StatusMessage = $"Framework test failed: {ex.Message}";
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                MessageService.Unsubscribe(ViewModelId);
                ThemeService.ThemeChanged -= OnFrameworkThemeChanged;
                LayoutService.LayoutChanged -= OnFrameworkLayoutChanged;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}