# Theme System Documentation

## Overview
The WPF Docking Demo now includes a comprehensive theme system that supports Light and Dark themes with automatic persistence and system theme detection.

## Features

### ?? **Theme Types**
- **Light Theme**: Clean, modern light interface with subtle shadows and borders
- **Dark Theme**: Professional dark interface similar to Visual Studio Code
- **System Theme**: Automatically follows Windows system theme preference (planned)

### ??? **Theme Controls**
- **Menu Integration**: Theme menu in the main menu bar
- **Status Bar Widget**: Real-time theme selector with dropdown and toggle button
- **Keyboard Shortcuts**: Quick theme switching via menu accelerators

### ?? **Persistence**
- Automatically saves theme preference to `%AppData%\Demo_Wpf\theme-settings.json`
- Remembers theme choice across application restarts
- Falls back to system theme detection on first launch

## Usage

### Switching Themes
1. **Via Menu**: `Theme ? Light Theme` or `Theme ? Dark Theme`
2. **Via Status Bar**: Use the dropdown in the status bar
3. **Quick Toggle**: Click the ?? button in status bar or use `Theme ? Toggle Theme`

### Programmatic Theme Changes
```csharp
// Access the theme service
var themeService = App.ThemeService;

// Switch to dark theme
themeService.SetTheme(ThemeType.Dark);

// Switch to light theme  
themeService.SetTheme(ThemeType.Light);

// Subscribe to theme changes
themeService.ThemeChanged += (sender, e) => {
    Console.WriteLine($"Theme changed from {e.OldTheme.Name} to {e.NewTheme.Name}");
};
```

## Architecture

### **Core Components**

#### `IThemeService` Interface
Defines the contract for theme management operations.

#### `ThemeService` Class
- Manages theme registration and application
- Handles persistence to/from JSON files
- Provides theme change notifications
- Detects system theme preferences

#### Theme Models
- `Theme`: Represents a complete theme with resources
- `ThemeType`: Enumeration of available theme types
- `ThemeChangedEventArgs`: Event arguments for theme change notifications

### **Resource Management**
Themes are defined as resource dictionaries containing:
- **Color Brushes**: All colors are defined as `SolidColorBrush` resources
- **Dynamic Resources**: All controls use `{DynamicResource}` bindings
- **Style Inheritance**: Controls inherit from themed base styles

### **Themed Resources**

#### Background Colors
```xml
<SolidColorBrush x:Key="PrimaryBackgroundBrush" Color="#1E1E1E"/>
<SolidColorBrush x:Key="SecondaryBackgroundBrush" Color="#252526"/>
<SolidColorBrush x:Key="TertiaryBackgroundBrush" Color="#2D2D30"/>
```

#### Text Colors
```xml
<SolidColorBrush x:Key="PrimaryTextBrush" Color="#FFFFFF"/>
<SolidColorBrush x:Key="SecondaryTextBrush" Color="#CCCCCC"/>
<SolidColorBrush x:Key="AccentTextBrush" Color="#569CD6"/>
```

#### Control-Specific Colors
- `ButtonBackgroundBrush`, `ButtonHoverBackgroundBrush`, `ButtonPressedBackgroundBrush`
- `MenuBackgroundBrush`, `MenuHoverBackgroundBrush`
- `TabBackgroundBrush`, `TabSelectedBackgroundBrush`, `TabHoverBackgroundBrush`
- `DockingPanelHeaderBrush`, `DockingPanelContentBrush`
- `DropZoneBackgroundBrush`, `DropZoneBorderBrush`

## Extending the Theme System

### Adding New Themes
```csharp
var customTheme = new Theme
{
    Name = "Custom",
    Type = ThemeType.Custom, // Add new enum value
    Resources = new Dictionary<string, object>
    {
        ["PrimaryBackgroundBrush"] = "#FF5733",
        ["PrimaryTextBrush"] = "#FFFFFF",
        // ... other resources
    }
};

App.ThemeService.RegisterTheme(customTheme);
```

### Adding New Themed Resources
1. Add the resource key to both Light and Dark themes in `ThemeService`
2. Reference the resource in XAML using `{DynamicResource ResourceKey}`
3. The theme system will automatically apply the correct value

### Creating Themed Controls
```xml
<UserControl>
    <Border Background="{DynamicResource PrimaryBackgroundBrush}">
        <TextBlock Foreground="{DynamicResource PrimaryTextBrush}"
                   Text="This text will adapt to theme changes"/>
    </Border>
</UserControl>
```

## Best Practices

### ? **Do**
- Always use `{DynamicResource}` for theme-related bindings
- Inherit from themed base styles when creating custom styles
- Test both light and dark themes during development
- Use semantic resource names (e.g., `PrimaryTextBrush` not `WhiteBrush`)

### ? **Don't**
- Use hardcoded colors in XAML or code-behind
- Use `{StaticResource}` for theme resources (they won't update)
- Create styles that override theme colors without fallbacks

## Files Structure
```
Demo-Wpf/
??? Interfaces/
?   ??? IThemeService.cs
??? Models/
?   ??? ThemeModels.cs
??? Services/
?   ??? ThemeService.cs
??? Themes/
?   ??? ThemeResources.xaml
??? Controls/
?   ??? ThemeSelector.xaml(.cs)
??? ViewModels/
    ??? MainWindowViewModel.cs (theme integration)
```

## Configuration Storage
Theme preferences are stored in:
- **Path**: `%AppData%\Demo_Wpf\theme-settings.json`
- **Format**: JSON with theme type information
- **Fallback**: System theme detection via Windows Registry

The theme system provides a robust, extensible foundation for creating visually appealing applications that respect user preferences and system settings.