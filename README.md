# ?? HCWpfFramework - Work in Progress ??

> **?? This project is currently under active development and is not yet ready for production use. Features and APIs are subject to change without notice.**

## HCWpfFramework

A modern WPF framework designed to provide a comprehensive foundation for building professional desktop applications with minimal boilerplate code.

## ?? Project Goals

- **Rapid Development**: Get from idea to working application faster
- **Modern UI**: Built-in theming system with Light/Dark theme support
- **Flexible Layout**: Advanced docking system for IDE-style applications
- **MVVM Ready**: Complete MVVM architecture support with base classes
- **Extensible**: Plugin-friendly design for custom functionality

## ??? Current Features (In Development)

- ? **Theme System**: Light/Dark theme switching with dynamic resource management
- ? **Layout Management**: Multiple layout configurations (Single, TopBottom, ThreePane, FourPane)
- ? **Docking Panels**: Advanced docking system with drag-and-drop support
- ? **MVVM Infrastructure**: ViewModelBase, RelayCommand, and messaging services
- ? **Dependency Injection**: Built-in service container support
- ?? **Background Services**: Long-running task management (in progress)
- ?? **Window Management**: Multi-window applications with shared services (planned)

## ?? Quick Start

```csharp
// Inherit from MainViewModelBase for instant framework integration
public class MainViewModel : MainViewModelBase
{
    public MainViewModel(IMessageService messageService, IThemeService themeService, ILayoutService layoutService) 
        : base(messageService, themeService, layoutService)
    {
        Title = "My Application";
        // Framework layout and theming automatically initialized
    }
}
```

## ?? Project Structure

```
HCWpfFramework/
??? Controls/          # Custom WPF controls
??? Services/          # Framework services (Theme, Layout, Messaging)
??? ViewModels/        # MVVM base classes
??? Themes/            # Theme resources and styling
??? Interfaces/        # Service contracts
??? Models/            # Data models and DTOs
??? Converters/        # Value converters
??? Commands/          # Command implementations
??? Documentation/     # Technical documentation

SampleApp/
??? Ultimate simplicity demo showing framework usage
```

## ?? Features Overview

### Theme System
- **Light/Dark Themes**: Seamless switching between themes
- **Dynamic Resources**: All UI elements update automatically
- **Extensible**: Add custom themes easily
- **System Integration**: Respects Windows theme preferences

### Layout Management
- **Multiple Layouts**: Single, TopBottom, ThreePane, FourPane configurations
- **Docking Panels**: Drag-and-drop window management
- **Persistence**: Layout preferences saved and restored
- **Customizable**: Override layouts for application-specific needs

### MVVM Architecture
- **ViewModelBase**: INotifyPropertyChanged implementation with helpers
- **RelayCommand**: Flexible command implementation
- **Messaging Service**: Decoupled component communication
- **Service Integration**: Automatic dependency injection support

## ??? Development Status

This framework is being actively developed. Current focus areas:

- [ ] **Stability**: Fixing layout and theme synchronization issues
- [ ] **Documentation**: Comprehensive guides and API documentation
- [ ] **Testing**: Unit tests and integration test coverage
- [ ] **Performance**: Optimization and memory management
- [ ] **Examples**: More sample applications and use cases

## ?? Contributing

This project is in early development. Contributions, feedback, and suggestions are welcome!

## ?? License

This project is licensed under the MIT License - see the LICENSE file for details.

## ?? Links

- **Repository**: https://github.com/hosercoder/HCWpfFramework
- **Issues**: Report bugs and request features via GitHub Issues
- **Documentation**: See `/Documentation` folder for technical details

---

**Note**: This README will be updated as the framework evolves. Check back frequently for the latest information and features.