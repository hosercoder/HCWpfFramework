# Layout Inheritance System

The HCWpfFramework provides a comprehensive layout inheritance system that allows applications to leverage pre-built layouts while maintaining the flexibility to customize them as needed.

## Overview

Instead of recreating layouts from scratch, applications can now inherit from the framework's `MainViewModelBase` class to automatically get:

- **Default Framework Layouts**: Pre-configured docking panels with standard windows
- **Theme Integration**: Automatic theme-aware styling
- **Layout Management**: Built-in layout switching and persistence
- **Customization Hooks**: Override methods to add application-specific content

## Quick Start

### 1. Inherit from MainViewModelBase

Instead of inheriting from `ViewModelBase`, inherit from `MainViewModelBase`:

```csharp
public class MainViewModel : MainViewModelBase
{
    public MainViewModel(IMessageService messageService, IThemeService themeService, ILayoutService layoutService) 
        : base(messageService, themeService, layoutService)
    {
        Title = "My Application";
        // Framework layout is automatically initialized
    }
}
```

### 2. Register Layout Service

Update your DI registration to include the layout service:

```csharp
services.AddHCWpfFramework(); // Already includes ILayoutService
```

### 3. Customize Layout (Optional)

Override the `CustomizeLayout` method to add application-specific windows:

```csharp
protected override void CustomizeLayout()
{
    // Add custom windows to existing framework layout
    CenterPanelWindows.Add(new DockableWindow
    {
        Id = "my-custom-window",
        Title = "My Custom Window",
        Description = "Application-specific content...",
        CanClose = true,
        CanFloat = true
    });
}
```

## Framework-Provided Default Windows

The layout service automatically creates these default windows:

### Left Panel
- **Explorer**: Framework structure and navigation
- **Toolbox**: Available framework components and controls

### Right Panel  
- **Properties**: Property inspector for framework elements

### Center Panel
- **Welcome**: Framework introduction and features overview

### Bottom Panel
- **Output**: Framework status and system messages
- **Messages**: Messaging system activity and logs

## Layout Types

The framework supports multiple layout configurations:

- **Single**: Single main content area
- **TopBottom**: Horizontal split layout
- **ThreePane**: Traditional IDE-style layout with left, center, and right panels

## Built-in Commands

When inheriting from `MainViewModelBase`, you automatically get these framework commands:

- `ChangeLayoutCommand`: Switch between layout types
- `ResetLayoutCommand`: Reset to default framework layout
- `SaveLayoutCommand`: Save current layout configuration
- `LoadLayoutCommand`: Load saved layout configuration

## Menu Integration

Add layout management to your menu:

```xml
<MenuItem Header=\"Layout\">
    <MenuItem Header=\"Single Pane\" Command=\"{Binding ChangeLayoutCommand}\" CommandParameter=\"Single\"/>
    <MenuItem Header=\"Top/Bottom Split\" Command=\"{Binding ChangeLayoutCommand}\" CommandParameter=\"TopBottom\"/>
    <MenuItem Header=\"Three Pane\" Command=\"{Binding ChangeLayoutCommand}\" CommandParameter=\"ThreePane\"/>
    <Separator/>
    <MenuItem Header=\"Save Layout\" Command=\"{Binding SaveLayoutCommand}\"/>
    <MenuItem Header=\"Load Layout\" Command=\"{Binding LoadLayoutCommand}\"/>
</MenuItem>
```

## Customization Hooks

### Override Methods

```csharp
protected override void CustomizeLayout()
{
    // Add application-specific windows to framework layout
}

protected override void InitializeCustomCommands()
{
    // Add application-specific commands
    MyCustomCommand = new RelayCommand(ExecuteMyCustomAction);
}

protected override void HandleCustomMessage(IMessage message)
{
    // Handle application-specific message types
}

protected override void OnLayoutChanged(LayoutChangedEventArgs e)
{
    // React to layout changes
    base.OnLayoutChanged(e); // Call base to get default behavior
    StatusMessage = $\"Layout changed to {e.NewLayout}\";
}

protected override void OnThemeChanged(ThemeChangedEventArgs e)
{
    // React to theme changes  
    base.OnThemeChanged(e); // Call base to get default behavior
    StatusMessage = $\"Theme changed to {e.NewTheme.Name}\";
}
```

### Custom Window Factories

For more advanced scenarios, register custom window factories:

```csharp
// In your application startup
var layoutService = serviceProvider.GetService<ILayoutService>();

layoutService.RegisterWindowFactory(DockingArea.Left, () => new[]
{
    new DockableWindow 
    { 
        Id = \"my-explorer\", 
        Title = \"My Explorer\",
        Description = \"Application-specific explorer content...\"
    }
});
```

## Benefits

### ? Consistency
- All applications using the framework have a consistent base layout
- Familiar user experience across different applications

### ? Rapid Development  
- No need to recreate common layout patterns
- Focus on application-specific functionality

### ? Maintainability
- Framework layout improvements benefit all applications
- Centralized layout logic reduces duplicate code

### ? Flexibility
- Override system allows complete customization when needed
- Can selectively replace or extend framework components

### ? Best Practices
- Built-in theme integration
- Proper MVVM patterns
- Service-based architecture

## Migration from Custom Layouts

If you have existing custom layouts, migration is straightforward:

1. **Change inheritance**: `ViewModelBase` ? `MainViewModelBase`
2. **Update constructor**: Add `ILayoutService` parameter
3. **Move custom windows**: From constructor to `CustomizeLayout()` override
4. **Remove boilerplate**: Delete manual layout initialization code
5. **Update DI registration**: Ensure `ILayoutService` is registered

The framework will handle all the standard layout functionality, and you can focus on your application-specific features!

## Example: SampleApp Implementation

See `SampleApp\ViewModels\MainViewModel.cs` for a complete example of how to use the layout inheritance system.