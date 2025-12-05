using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using HCWpfFramework.ViewModels;
using HCWpfFramework.Commands;
using HCWpfFramework.Interfaces;
using HCWpfFramework.Models;
using HCWpfFramework.Services;

namespace SampleApp.ViewModels
{
    /// <summary>
    /// Sample application ViewModel that demonstrates complete framework inheritance
    /// This shows the minimal code needed when leveraging the framework fully
    /// </summary>
    public class SampleMainViewModel : MainViewModelBase
    {
        private int _dynamicWindowCounter = 0;
        private int _messageCount = 0;

        public SampleMainViewModel(IMessageService messageService, IThemeService themeService, ILayoutService layoutService) 
            : base(messageService, themeService, layoutService)
        {
            // Set application title
            Title = "HCWpfFramework Sample Application";
            
            // Initialize with framework status
            StatusMessage = $"Framework fully inherited! Current theme: {ThemeService.CurrentTheme.Name}";
            
            // Set custom menu content for sample-specific menu items
            SetCustomMenuContent();
            
            // Set custom status content
            SetCustomStatusContent();
        }

        #region Sample-Specific Properties

        public int MessageCount
        {
            get => _messageCount;
            set => SetProperty(ref _messageCount, value);
        }

        #endregion

        #region Sample Commands (Minimal set for demonstration)

        public ICommand SendSampleMessageCommand { get; private set; } = null!;
        public ICommand AddDynamicWindowCommand { get; private set; } = null!;
        public ICommand ShowFrameworkInfoCommand { get; private set; } = null!;

        protected override void InitializeCustomCommands()
        {
            // Only define sample-specific commands
            SendSampleMessageCommand = new RelayCommand(ExecuteSendSampleMessage);
            AddDynamicWindowCommand = new RelayCommand(ExecuteAddDynamicWindow);
            ShowFrameworkInfoCommand = new RelayCommand(ExecuteShowFrameworkInfo);
        }

        #endregion

        #region Command Implementations

        private void ExecuteSendSampleMessage()
        {
            MessageCount++;
            var sampleMessage = new
            {
                Type = "Sample Demonstration",
                Message = $"This is sample message #{MessageCount}",
                Timestamp = DateTime.Now,
                Features = new[] { "Framework inheritance", "Minimal code", "Full functionality" }
            };

            MessageService.SendMessage(MessageType.Information, ViewModelId, sampleMessage);
            StatusMessage = $"Sample message #{MessageCount} sent via framework messaging system";
        }

        private void ExecuteAddDynamicWindow()
        {
            _dynamicWindowCounter++;
            
            var window = new DockableWindow
            {
                Id = $"dynamic-window-{_dynamicWindowCounter}",
                Title = $"Dynamic Window {_dynamicWindowCounter}",
                Description = $"?? Dynamically Added Window #{_dynamicWindowCounter}\\n\\n" +
                             $"? This demonstrates:\\n" +
                             $"• Framework layout inheritance\\n" +
                             $"• Runtime window creation\\n" +
                             $"• Automatic docking integration\\n" +
                             $"• Theme-aware styling\\n\\n" +
                             $"?? Added at: {DateTime.Now:HH:mm:ss}\\n" +
                             $"?? Total dynamic windows: {_dynamicWindowCounter}\\n\\n" +
                             $"The framework handles all the complexity - you just add content!",
                CanClose = true,
                CanFloat = true
            };

            // Add to center panel
            CenterPanelWindows.Add(window);
            StatusMessage = $"Added dynamic window to center panel (Total: {_dynamicWindowCounter})";
        }

        private void ExecuteShowFrameworkInfo()
        {
            var info = new
            {
                Framework = "HCWpfFramework",
                Version = "1.0.0",
                Features = new[]
                {
                    "Complete layout inheritance",
                    "Automatic theme management", 
                    "Built-in messaging system",
                    "Docking panel system",
                    "MVVM pattern support",
                    "Dependency injection ready"
                },
                Benefits = new[]
                {
                    "95% less boilerplate code",
                    "Consistent UI across applications",
                    "Rapid development cycle",
                    "Professional appearance",
                    "Extensible architecture"
                }
            };

            MessageService.SendMessage(MessageType.Information, ViewModelId, info);
            StatusMessage = "Framework information displayed - see Messages panel";
        }

        #endregion

        #region Framework Customization (Minimal overrides)

        protected override void CustomizeLayout()
        {
            // Add sample-specific demonstration windows
            AddSampleDemonstrationWindows();
        }

        protected override void HandleCustomMessage(IMessage message)
        {
            // Count all messages for demonstration
            if (message.MessageType == MessageType.Information)
            {
                StatusMessage = $"Framework message received: {message.MessageType} at {message.Timestamp:HH:mm:ss}";
            }
        }

        protected override void OnThemeChanged(ThemeChangedEventArgs e)
        {
            base.OnThemeChanged(e);
            StatusMessage = $"?? Theme automatically changed to {e.NewTheme.Name} by framework!";
        }

        protected override void OnLayoutChanged(LayoutChangedEventArgs e)
        {
            base.OnLayoutChanged(e);
            StatusMessage = $"??? Layout automatically changed to {e.NewLayout} by framework!";
        }

        #endregion
        
        #region Menu Customization
        
        private void SetCustomMenuContent()
        {
            // Create sample-specific menu items using framework styling
            var sampleMenu = new System.Windows.Controls.MenuItem
            {
                Header = "Sample"
            };
            sampleMenu.SetResourceReference(System.Windows.Controls.MenuItem.StyleProperty, "ThemedMenuItemStyle");
            
            var sendMessageItem = new System.Windows.Controls.MenuItem
            {
                Header = "Send Sample Message",
                Command = SendSampleMessageCommand
            };
            sendMessageItem.SetResourceReference(System.Windows.Controls.MenuItem.StyleProperty, "ThemedSubmenuItemStyle");
            
            var addWindowItem = new System.Windows.Controls.MenuItem
            {
                Header = "Add Dynamic Window",
                Command = AddDynamicWindowCommand
            };
            addWindowItem.SetResourceReference(System.Windows.Controls.MenuItem.StyleProperty, "ThemedSubmenuItemStyle");
            
            var separator = new System.Windows.Controls.Separator();
            separator.SetResourceReference(System.Windows.Controls.Separator.StyleProperty, "ThemedSeparatorStyle");
            
            var showInfoItem = new System.Windows.Controls.MenuItem
            {
                Header = "Show Framework Info",
                Command = ShowFrameworkInfoCommand
            };
            showInfoItem.SetResourceReference(System.Windows.Controls.MenuItem.StyleProperty, "ThemedSubmenuItemStyle");
            
            sampleMenu.Items.Add(sendMessageItem);
            sampleMenu.Items.Add(addWindowItem);
            sampleMenu.Items.Add(separator);
            sampleMenu.Items.Add(showInfoItem);
            
            CustomMenuContent = sampleMenu;
        }
        
        private void SetCustomStatusContent()
        {
            // Create sample-specific status information
            var statusPanel = new StackPanel { Orientation = Orientation.Horizontal };
            
            var messagesLabel = new TextBlock { Text = "Messages: " };
            messagesLabel.SetResourceReference(TextBlock.StyleProperty, "ThemedSecondaryTextBlockStyle");
            
            var messagesCount = new TextBlock();
            messagesCount.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("MessageCount") { Source = this });
            messagesCount.SetResourceReference(TextBlock.StyleProperty, "ThemedTextBlockStyle");
            messagesCount.FontWeight = FontWeights.SemiBold;
            
            statusPanel.Children.Add(messagesLabel);
            statusPanel.Children.Add(messagesCount);
            
            CustomStatusContent = statusPanel;
        }
        
        #endregion

        #region Private Methods

        private void AddSampleDemonstrationWindows()
        {
            // Add a welcome window that explains the framework inheritance
            CenterPanelWindows.Add(new DockableWindow
            {
                Id = "sample-inheritance-demo",
                Title = "Framework Inheritance Demo",
                Description = "?? Welcome to HCWpfFramework Sample!\\n\\n" +
                             "? What you get automatically:\\n" +
                             "• Professional layout with Explorer, Properties, Output panels\\n" +
                             "• Theme switching (Light/Dark) with persistence\\n" +
                             "• Layout management (Single/TopBottom/ThreePane)\\n" +
                             "• Thread-safe messaging system\\n" +
                             "• Docking and floating windows\\n" +
                             "• MVVM pattern with ViewModelBase\\n" +
                             "• Dependency injection integration\\n\\n" +
                             "?? This sample ViewModel has only ~150 lines of code\\n" +
                             "vs. ~500+ lines without framework inheritance!\\n\\n" +
                             "?? Try:\\n" +
                             "• Switch themes using the menu\\n" +
                             "• Change layouts (Layout menu)\\n" +
                             "• Add dynamic windows\\n" +
                             "• Send sample messages\\n\\n" +
                             "All functionality comes from the framework!",
                CanClose = false, // Keep this window for reference
                CanFloat = true
            });

            // Add sample demonstration controls
            BottomPanelWindows.Add(new DockableWindow
            {
                Id = "sample-actions",
                Title = "Sample Actions",
                Description = "?? Sample Application Actions\\n\\n" +
                             "Try these framework-integrated features:\\n\\n" +
                             "?? Send Sample Message:\\n" +
                             "• Tests the messaging system\\n" +
                             "• Shows framework message handling\\n" +
                             "• Updates status automatically\\n\\n" +
                             "? Add Dynamic Window:\\n" +
                             "• Creates windows at runtime\\n" +
                             "• Demonstrates docking integration\\n" +
                             "• Shows theme inheritance\\n\\n" +
                             "?? Show Framework Info:\\n" +
                             "• Displays framework capabilities\\n" +
                             "• Demonstrates structured messaging\\n" +
                             "• Shows development benefits\\n\\n" +
                             $"?? Current Status:\\n" +
                             $"• Theme: {ThemeService.CurrentTheme.Name}\\n" +
                             $"• Layout: {CurrentLayoutType}\\n" +
                             $"• Messages Sent: {MessageCount}",
                CanClose = true,
                CanFloat = true
            });
        }

        #endregion
    }
}