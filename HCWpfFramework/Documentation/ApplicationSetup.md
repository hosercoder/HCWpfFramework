# HCWpfFramework - Application Setup Guide

This guide shows the different ways to set up a HCWpfFramework application, from the simplest one-liner to more customizable approaches.

## ?? **Method 1: Standard WPF App.xaml Approach (Recommended)**

This is the standard WPF approach using `App.xaml` and `App.xaml.cs`:

### App.xaml.cs
```csharp
using System.Windows;
using HCWpfFramework;
using SampleApp.ViewModels;
using System.Diagnostics;

namespace SampleApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Call base first to initialize the application
                base.OnStartup(e);
                
                // One method call - framework handles everything!
                var mainWindow = HCWpfApplication.RunApplication<UltimateSimpleViewModel>();
                
                Debug.WriteLine("Application started successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application startup failed: {ex.Message}", "Startup Error");
                Shutdown(1);
            }
        }
    }
}
```

### Your ViewModel
```csharp
public class UltimateSimpleViewModel : MainViewModelBase
{
    public UltimateSimpleViewModel(IMessageService messageService, IThemeService themeService, ILayoutService layoutService, IWindowFactoryService windowFactoryService) 
        : base(messageService, themeService, layoutService)
    {
        Title = "My HCWpf Application";
        // Framework handles the rest!
    }
}
```

## ?? **Method 2: Console App / No App.xaml Approach**

For converting console apps or when you don't want App.xaml:

### Program.cs (Main method)
```csharp
using System;
using System.Windows;
using HCWpfFramework;
using MyApp.ViewModels;

namespace MyApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // One line - complete WPF application!
            HCWpfApplication.CreateAndRunApplication<MyMainViewModel>();
        }
    }
}
```

## ?? **Method 3: Custom Configuration**

When you need to configure services or add custom assemblies:

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    
    var mainWindow = HCWpfApplication.RunApplication<MyViewModel>(
        services => {
            // Add your custom services
            services.AddSingleton<IMyCustomService, MyCustomService>();
            services.AddDbContext<MyDbContext>();
        },
        // Additional assemblies to scan for window factories
        Assembly.GetAssembly(typeof(MyPluginWindowFactory))
    );
}
```

## ?? **Method 4: Manual Service Provider Access**

When you need more control:

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    
    // Create the application infrastructure
    var serviceProvider = HCWpfApplication.CreateApplication<MyViewModel>(
        services => {
            // Configure services
        }
    );
    
    // Access services manually if needed
    var themeService = serviceProvider.GetRequiredService<IThemeService>();
    themeService.SetTheme(ThemeType.Dark);
    
    // Create and show window
    var mainWindow = HCWpfApplication.RunApplication<MyViewModel>();
}
```

## ?? **What You Get Automatically**

All approaches provide:
- ? **Professional 3-pane layout** with Explorer, Properties, Output panels
- ? **Theme system** (Light/Dark) with persistence
- ? **Layout management** (Single/TopBottom/ThreePane)
- ? **Docking system** with drag & drop
- ? **Messaging system** for inter-component communication
- ? **Window factory auto-discovery** from your assemblies
- ? **MVVM architecture** with ViewModelBase and commands
- ? **Dependency injection** ready

## ??? **Creating Custom Windows**

Just implement the `IWindowFactory` interface:

```csharp
public class MyCustomWindowFactory : WindowFactoryBase
{
    public override string WindowId => "my-custom-window";
    public override string DisplayName => "My Custom Window";
    public override string Category => "MyApp";
    public override DockingArea DefaultDockingArea => DockingArea.Left;
    
    public override DockableWindow CreateWindow()
    {
        return CreateBasicWindow("My Window", "My content here!");
    }
}
```

The framework automatically discovers and integrates all `IWindowFactory` implementations!

## ?? **Migration from Custom WPF Apps**

To migrate existing WPF applications:

1. **Change ViewModel inheritance**: `ViewModelBase` ? `MainViewModelBase`
2. **Replace startup code**: Use `HCWpfApplication.RunApplication<T>()`
3. **Convert windows to factories**: Implement `IWindowFactory` for each window
4. **Remove layout code**: Delete manual layout/theme/docking code

Result: 70-90% less code with professional appearance and advanced functionality!

## ?? **Troubleshooting**

### "Cannot create more than one Application instance"
- Use `RunApplication<T>()` (works with existing App) instead of `CreateAndRunApplication<T>()` (creates new App)

### Window factories not found
- Ensure classes implement `IWindowFactory` and have parameterless constructors
- Check that assemblies are being scanned correctly

### Services not available
- Make sure services are registered in the `configureServices` callback
- Use `HCWpfApplication.GetServiceProvider()` to access services from anywhere

The framework handles all the complexity - you focus on your business logic! ??