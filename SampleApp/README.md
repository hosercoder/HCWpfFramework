# HCWpfFramework Sample Application - Complete Framework Inheritance

This sample demonstrates the **complete inheritance** approach to using the HCWpfFramework, showing how applications can leverage the framework with minimal code while getting full functionality.

## ?? Key Differences: Before vs After

### Before (Custom Implementation)
- **MainViewModel.cs**: ~500 lines of layout management code
- **MainWindow.xaml**: ~200 lines of layout XAML
- **MainWindow.xaml.cs**: ~50 lines of event handling
- **Total**: ~750 lines to recreate framework functionality

### After (Complete Framework Inheritance)  
- **SampleMainViewModel.cs**: ~200 lines (includes menu/status customization)
- **SampleMainWindow.xaml**: **0 lines** (no XAML file needed!)
- **SampleMainWindow.xaml.cs**: ~30 lines inheriting from FrameworkMainWindow
- **Total**: ~230 lines with SIGNIFICANTLY MORE functionality

## ? What You Get Automatically

By inheriting from `MainViewModelBase`, you automatically get:

### ??? Layout Management
- **Professional 3-pane layout** (Left/Center/Right panels)
- **Built-in Explorer, Properties, Output panels** with default content
- **Layout switching** (Single/TopBottom/ThreePane) with persistence
- **Docking and floating windows** with drag & drop

### ?? Theme System  
- **Light/Dark theme switching** with live updates
- **Theme persistence** across application restarts
- **System theme detection** on first launch
- **Complete UI styling** for all controls

### ?? Messaging System
- **Thread-safe messaging** between components
- **Event-driven architecture** with automatic subscriptions
- **Message type categorization** (Info/Warning/Error)
- **Subscriber management** and cleanup

### ?? Framework Services
- **Dependency injection** ready with automatic service registration
- **MVVM pattern** with ViewModelBase and RelayCommand
- **Background services** for long-running tasks
- **Resource management** and disposal patterns

## ?? Sample Features Demonstrated

This sample shows how to:

### ?? Minimal Code Implementation
```csharp
public class SampleMainViewModel : MainViewModelBase
{
    public SampleMainViewModel(IMessageService messageService, IThemeService themeService, ILayoutService layoutService) 
        : base(messageService, themeService, layoutService)
    {
        Title = "HCWpfFramework Sample Application";
    }
    
    protected override void CustomizeLayout()
    {
        // Add only your application-specific windows
        CenterPanelWindows.Add(myCustomWindow);
    }
}
```

### ??? Custom Commands
- **SendSampleMessageCommand**: Demonstrates framework messaging
- **AddDynamicWindowCommand**: Shows runtime window creation  
- **ShowFrameworkInfoCommand**: Displays framework capabilities

### ?? Framework Integration
- **Automatic layout inheritance** from framework defaults
- **Custom window addition** via `CustomizeLayout()` override
- **Message handling** via `HandleCustomMessage()` override
- **Theme/Layout change responses** via event overrides

## ????? Getting Started

1. **Run the application** - see the complete framework layout
2. **Try theme switching** - use the Theme menu or selector
3. **Test layout changes** - use Layout menu (Single/TopBottom/ThreePane)
4. **Send sample messages** - use Sample menu items
5. **Add dynamic windows** - see runtime docking integration

## ?? Code Reduction Benefits

| Feature | Custom Code | Framework Inheritance | Reduction |
|---------|-------------|---------------------|-----------|
| Layout Management | ~200 lines | 0 lines (inherited) | **100%** |
| Theme System | ~150 lines | 0 lines (inherited) | **100%** |
| Messaging Setup | ~100 lines | 0 lines (inherited) | **100%** |
| MVVM Infrastructure | ~50 lines | 0 lines (inherited) | **100%** |
| **Total** | **~500 lines** | **~150 lines** | **?? 70% reduction** |

## ?? Professional Appearance

The framework provides:
- **Consistent styling** across all UI elements
- **Professional color schemes** with accessibility considerations
- **Responsive layouts** that adapt to window resizing
- **Visual feedback** for user interactions (hover, selection, etc.)
- **Icon integration** and visual hierarchy

## ?? Development Workflow

1. **Inherit** from `MainViewModelBase`
2. **Override** `CustomizeLayout()` to add your windows
3. **Define** your application-specific commands in `InitializeCustomCommands()`
4. **Handle** custom messages in `HandleCustomMessage()`
5. **Focus** on your business logic instead of UI infrastructure

## ?? Framework Philosophy

**"Write less, achieve more"** - The framework handles all the complex infrastructure so you can focus on what makes your application unique. Professional appearance and robust functionality come built-in.

This sample demonstrates that with proper framework inheritance, you can create sophisticated applications with minimal code while maintaining full customization capabilities.

## ?? Next Steps

- Explore the framework documentation in `HCWpfFramework\Documentation\`
- Try customizing the layout with your own windows
- Implement custom message types and handlers  
- Add application-specific menu items and commands
- Experiment with different layout configurations