# HCWpfFramework

A comprehensive WPF framework providing MVVM support, theming, messaging, docking controls, and other common WPF functionality to accelerate application development.

## Features

### ??? **MVVM Architecture Support**
- **ViewModelBase** - Base class with INotifyPropertyChanged implementation
- **RelayCommand** - Flexible command implementation for UI interactions
- **Property binding helpers** with automatic change notifications

### ?? **Comprehensive Theme System**
- **IThemeService** - Interface for theme management
- **Light/Dark themes** with extensive color palettes  
- **Dynamic theme switching** with live updates
- **System theme detection** and automatic switching
- **Customizable theme resources** for your application needs
- **ThemeSelector control** for easy theme switching UI

### ?? **Thread-Safe Messaging System**
- **IMessageService** - Interface for inter-component communication
- **ThreadSafeMessageService** - WPF Dispatcher-aware messaging
- **Broadcast and targeted messaging** capabilities
- **Event-driven architecture** for loose coupling
- **Background worker integration** with progress reporting

### ?? **Docking System**
- **DockingPanel** - Container for dockable content with drag-and-drop support
- **DockableContent** - Individual dockable windows with float/close functionality
- **Dynamic layouts** - Single, Top/Bottom Split, Three Pane configurations
- **Visual drop indicators** for better user experience
- **Floating window management** with automatic docking
- **Tab management** with close and reorder functionality

### ?? **Background Services**
- **BackgroundWorkerService** - Async task execution with progress reporting
- **Cancellation token support** for proper cleanup
- **Integration with messaging system** for status updates

### ?? **Utility Components**
- **Value converters** for data binding scenarios
- **Extension methods** for service registration
- **Bootstrapper class** for easy framework initialization

## Installation

### NuGet Package (Recommended)
```bash
Install-Package HCWpfFramework
```

### Manual Installation
1. Download the source code
2. Add reference to HCWpfFramework.dll in your project
3. Follow the setup instructions below

## Quick Start

### 1. Initialize the Framework

In your `App.xaml.cs`:

```csharp
using HCWpfFramework;
using HCWpfFramework.Extensions;
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Initialize the framework
        var services = new ServiceCollection();
        services.AddHCWpfFramework();
        
        ServiceProvider = services.BuildServiceProvider();
        
        // Or use the simple bootstrapper
        HCWpfFrameworkBootstrapper.Initialize();
        
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        HCWpfFrameworkBootstrapper.Cleanup();
        base.OnExit(e);
    }
}
```

### 2. Create a ViewModel

```csharp
using HCWpfFramework.ViewModels;
using HCWpfFramework.Commands;

public class MainViewModel : ViewModelBase
{
    private string _title = "My Application";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public RelayCommand SaveCommand { get; }

    public MainViewModel()
    {
        SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
    }

    private void ExecuteSave()
    {
        // Save logic here
        Title = "Saved!";
    }

    private bool CanExecuteSave()
    {
        return !string.IsNullOrEmpty(Title);
    }
}
```

### 3. Use Docking Controls

```xml
<Window xmlns:hc="clr-namespace:HCWpfFramework.Controls;assembly=HCWpfFramework">
    <Grid>
        <hc:DockingPanel x:Name="MainDockingPanel" 
                         PanelTitle="Main Panel"
                         DockableWindows="{Binding DockableWindows}"
                         WindowFloatRequested="OnWindowFloatRequested"
                         WindowCloseRequested="OnWindowCloseRequested" />
    </Grid>
</Window>
```

### 4. Add Theme Support

```xml
<Window xmlns:hc="clr-namespace:HCWpfFramework.Controls;assembly=HCWpfFramework">
    <Grid>
        <hc:ThemeSelector ThemeService="{Binding ThemeService}" 
                          HorizontalAlignment="Right" 
                          VerticalAlignment="Top" 
                          Margin="10"/>
    </Grid>
</Window>
```

### 5. Use Messaging System

```csharp
using HCWpfFramework.Interfaces;
using HCWpfFramework.Services;

public class MyViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;

    public MyViewModel(IMessageService messageService)
    {
        _messageService = messageService;
        _messageService.Subscribe("MyViewModel", HandleMessage);
    }

    private void HandleMessage(IMessage message)
    {
        // Handle received messages
    }

    private void SendNotification()
    {
        _messageService.SendMessage(MessageType.Information, "MyViewModel", "Hello World!");
    }
}
```

## Advanced Configuration

### Custom Theme Registration

```csharp
var themeService = serviceProvider.GetRequiredService<IThemeService>();

var customTheme = new Theme
{
    Name = "Custom",
    Type = ThemeType.Custom,
    Resources = new Dictionary<string, object>
    {
        ["PrimaryBackgroundBrush"] = "#FF2D2D30",
        ["PrimaryTextBrush"] = "#FFFFFF"
        // Add more custom colors
    }
};

themeService.RegisterTheme(customTheme);
themeService.SetTheme(customTheme);
```

### Service Registration Options

```csharp
services.AddHCWpfFramework(themeService =>
{
    // Configure theme service
    themeService.SetTheme(ThemeType.Dark);
});

// Or register services individually
services.AddHCWpfThemeService(ThemeType.Light);
services.AddHCWpfMessaging();
services.AddHCWpfBackgroundServices();
```

### Custom Background Worker

```csharp
public class MyCustomWorker : BackgroundWorkerService
{
    public MyCustomWorker(IMessageService messageService) 
        : base("MyWorker", messageService)
    {
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        // Your custom background work here
        for (int i = 0; i < 100; i++)
        {
            await Task.Delay(100, cancellationToken);
            // Report progress via messaging system
        }
    }
}
```

## Framework Architecture

```
HCWpfFramework/
??? Commands/              # Command implementations (RelayCommand)
??? Controls/             # Custom WPF controls (DockingPanel, ThemeSelector)
??? Converters/          # Value converters for data binding
??? Extensions/          # Service collection extension methods
??? Interfaces/          # Service interfaces (IThemeService, IMessageService)
??? Models/             # Data models and DTOs
??? Services/           # Core framework services
??? ViewModels/         # MVVM base classes
```

## Best Practices

### 1. **Dependency Injection**
Always use the service collection extensions to register framework services and inject them into your ViewModels.

### 2. **Message Subscription Cleanup**
Always unsubscribe from messages in your Dispose methods or when components are no longer needed.

### 3. **Theme Resource Naming**
Use the standard resource keys provided by the framework for consistent theming across your application.

### 4. **Async Operations**
Use BackgroundWorkerService for long-running operations to keep the UI responsive.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues, feature requests, or questions:
- Create an issue on GitHub
- Check the documentation and examples
- Review the code samples in the repository

## Changelog

### Version 1.0.0
- Initial release
- MVVM support with ViewModelBase and RelayCommand
- Theme system with light/dark themes
- Thread-safe messaging system
- Docking controls with drag-and-drop support
- Background worker services
- Dependency injection integration