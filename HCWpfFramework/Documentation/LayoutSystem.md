# HCWpfFramework Layout System - Complete Guide

## ?? **Fixed Issues and New Features**

### ? **Issues Resolved**
1. **Layout Switching**: Layout menu buttons now work correctly
2. **Theme System**: Light/Dark theme switching is now functional
3. **Proper Layout Display**: Each layout now shows the correct panels
4. **Four Panel Layout**: Added new FourPane layout option

### ?? **Available Layouts**

#### **1. Single Pane Layout**
- **Description**: Single main content area
- **Use Case**: Simple applications, document viewers
- **Panels**: Center only
- **Command**: `ChangeLayoutCommand` with parameter `"Single"`

#### **2. Top/Bottom Layout** 
- **Description**: Horizontal split with main content and output
- **Use Case**: Development environments, data analysis
- **Panels**: Center (top) + Bottom (output)
- **Command**: `ChangeLayoutCommand` with parameter `"TopBottom"`

#### **3. Three Pane Layout** *(Default)*
- **Description**: Traditional IDE layout with left, center, right panels
- **Use Case**: Development environments, complex applications
- **Panels**: Left (Explorer) + Center (Main) + Right (Properties)
- **Command**: `ChangeLayoutCommand` with parameter `"ThreePane"`

#### **4. Four Pane Layout** *(New!)*
- **Description**: Full IDE layout with all panels visible
- **Use Case**: Professional development environments
- **Panels**: Left (Explorer) + Center (Main) + Bottom (Output) + Right (Properties)
- **Command**: `ChangeLayoutCommand` with parameter `"FourPane"`

## ?? **Implementation Details**

### **Dynamic Layout Switching**
The framework now uses DataTriggers to dynamically show/hide layout panels based on `CurrentLayoutType`:

```xml
<DataTrigger Binding="{Binding CurrentLayoutType}" Value="{x:Static models:LayoutType.ThreePane}">
    <Setter Property="Visibility" Value="Visible"/>
</DataTrigger>
```

### **Layout Commands**
All layout commands are automatically available in `MainViewModelBase`:

```csharp
// Change layout programmatically
ChangeLayoutCommand.Execute("FourPane");

// Or use the enum directly
ExecuteChangeLayout(LayoutType.FourPane);
```

### **Menu Integration**
The framework provides complete menu integration:

```xml
<MenuItem Header="Layout" Style="{StaticResource ThemedMenuItemStyle}">
    <MenuItem Header="Single Pane" Command="{Binding ChangeLayoutCommand}" CommandParameter="Single"/>
    <MenuItem Header="Top/Bottom Split" Command="{Binding ChangeLayoutCommand}" CommandParameter="TopBottom"/>
    <MenuItem Header="Three Pane" Command="{Binding ChangeLayoutCommand}" CommandParameter="ThreePane"/>
    <MenuItem Header="Four Pane" Command="{Binding ChangeLayoutCommand}" CommandParameter="FourPane"/>
    <Separator/>
    <MenuItem Header="Reset Layout" Command="{Binding ResetLayoutCommand}"/>
    <MenuItem Header="Save Layout" Command="{Binding SaveLayoutCommand}"/>
    <MenuItem Header="Load Layout" Command="{Binding LoadLayoutCommand}"/>
</MenuItem>
```

## ?? **Theme System**

### **Working Theme Switching**
The theme selector now properly switches between Light and Dark themes:

- **Light Theme**: Clean, bright interface suitable for daytime use
- **Dark Theme**: Easy on the eyes for extended coding sessions
- **Auto-Save**: Theme preference is automatically saved and restored

### **Theme Integration**
```csharp
// Change theme programmatically
ThemeService.SetTheme(ThemeType.Dark);

// Theme changes are automatically applied to all UI elements
```

## ?? **Panel Configuration**

### **Default Panel Contents**

#### **Left Panel (Explorer)**
- Framework Explorer: Shows framework structure and navigation
- Toolbox: Framework components and controls

#### **Center Panel (Main Content)**
- Welcome: Framework introduction and features
- Application-specific content windows

#### **Right Panel (Properties)**
- Properties Inspector: For selected elements
- Configuration panels

#### **Bottom Panel (Output)**
- Output: Framework and application messages
- Messages: Messaging system activity
- Logs: Debug and diagnostic information

## ?? **Layout Persistence**

### **Automatic Save/Load**
```csharp
// Save current layout
SaveLayoutCommand.Execute();

// Load saved layout  
LoadLayoutCommand.Execute();

// Reset to framework defaults
ResetLayoutCommand.Execute();
```

### **Configuration Storage**
Layout preferences are stored in:
- **Location**: `%AppData%\HCWpfFramework\layout-config.json`
- **Format**: JSON configuration file
- **Auto-Load**: Automatically restored on application startup

## ?? **Usage Examples**

### **Basic Layout Switching**
```csharp
public class MyViewModel : MainViewModelBase
{
    protected override void CustomizeLayout()
    {
        // Start with four pane layout for power users
        ExecuteChangeLayout(LayoutType.FourPane);
        
        // Add custom windows to appropriate panels
        LeftPanelWindows.Add(myExplorerWindow);
        CenterPanelWindows.Add(myMainWindow);
        RightPanelWindows.Add(myPropertiesWindow);
        BottomPanelWindows.Add(myOutputWindow);
    }
}
```

### **Dynamic Layout Changes**
```csharp
// Switch to single pane for presentation mode
private void EnterPresentationMode()
{
    ExecuteChangeLayout(LayoutType.Single);
    StatusMessage = "Presentation mode active";
}

// Return to development layout
private void ExitPresentationMode() 
{
    ExecuteChangeLayout(LayoutType.FourPane);
    StatusMessage = "Development mode active";
}
```

## ?? **Result**

The framework now provides:

1. ? **Perfect Layout Control**: All 4 layout types work correctly
2. ? **Working Theme System**: Light/Dark themes switch properly  
3. ? **Functional Menus**: All menu commands work as expected
4. ? **Professional UI**: Clean, responsive interface
5. ? **Easy Customization**: Simple override points for applications
6. ? **State Persistence**: Layouts and themes are remembered

The application now behaves like a professional IDE with full layout management capabilities!