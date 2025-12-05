using HCWpfFramework.Interfaces;
using HCWpfFramework.Models;
using HCWpfFramework.ViewModels;

namespace SampleApp.WindowFactories
{
    /// <summary>
    /// Example window factory - this is all users need to create!
    /// Just implement IWindowFactory and the framework handles everything else
    /// </summary>
    public class WelcomeWindowFactory : WindowFactoryBase
    {
        public override string WindowId => "sample-welcome";
        public override string DisplayName => "Welcome";
        public override string Category => "Sample";
        public override DockingArea DefaultDockingArea => DockingArea.Center;
        public override bool AllowMultipleInstances => false;

        public override DockableWindow CreateWindow()
        {
            return CreateBasicWindow(
                "Welcome to Framework",
                "?? Welcome to HCWpfFramework!\\n\\n" +
                "? This window was created by a simple WindowFactory.\\n\\n" +
                "??? The framework handled:\\n" +
                "• Automatic discovery and registration\\n" +
                "• Layout placement and management\\n" +
                "• Drag & drop functionality\\n" +
                "• Theme integration\\n" +
                "• State persistence\\n\\n" +
                "????? You just implemented IWindowFactory!\\n\\n" +
                "?? Create as many window factories as you need,\\n" +
                "the framework will automatically discover and\\n" +
                "integrate them into your application.",
                canClose: false
            );
        }
    }

    public class DataExplorerFactory : WindowFactoryBase
    {
        public override string WindowId => "data-explorer";
        public override string DisplayName => "Data Explorer";
        public override string Category => "Data";
        public override DockingArea DefaultDockingArea => DockingArea.Left;

        public override DockableWindow CreateWindow()
        {
            return CreateBasicWindow(
                "Data Explorer",
                "?? Data Explorer Window\\n\\n" +
                "??? This demonstrates a data-focused window:\\n\\n" +
                "?? Project Data\\n" +
                "  ?? Dataset 1\\n" +
                "  ?? Dataset 2\\n" +
                "  ?? Dataset 3\\n\\n" +
                "?? Search and filter capabilities\\n" +
                "?? Data visualization tools\\n" +
                "?? Import/Export functions\\n\\n" +
                "?? Category: Data\\n" +
                "?? Default: Left Panel"
            );
        }
    }

    public class ToolsWindowFactory : WindowFactoryBase
    {
        public override string WindowId => "tools-palette";
        public override string DisplayName => "Tools Palette";
        public override string Category => "Tools";
        public override DockingArea DefaultDockingArea => DockingArea.Right;

        public override DockableWindow CreateWindow()
        {
            return CreateBasicWindow(
                "Tools Palette",
                "??? Tools Palette\\n\\n" +
                "?? Available Tools:\\n\\n" +
                "?? Configuration Tools\\n" +
                "• Settings Manager\\n" +
                "• Theme Editor\\n" +
                "• Layout Designer\\n\\n" +
                "?? Development Tools\\n" +
                "• Code Generator\\n" +
                "• Debug Console\\n" +
                "• Performance Monitor\\n\\n" +
                "?? Design Tools\\n" +
                "• Color Picker\\n" +
                "• Icon Library\\n" +
                "• Font Manager\\n\\n" +
                "?? Category: Tools\\n" +
                "?? Default: Right Panel"
            );
        }
    }

    public class LogViewerFactory : WindowFactoryBase
    {
        public override string WindowId => "log-viewer";
        public override string DisplayName => "Log Viewer";
        public override string Category => "Debug";
        public override DockingArea DefaultDockingArea => DockingArea.Bottom;

        public override DockableWindow CreateWindow()
        {
            return CreateBasicWindow(
                "Log Viewer",
                "?? Application Log Viewer\\n\\n" +
                $"?? {DateTime.Now:HH:mm:ss} - Framework initialized\\n" +
                $"?? {DateTime.Now.AddSeconds(-30):HH:mm:ss} - Theme service started\\n" +
                $"?? {DateTime.Now.AddSeconds(-25):HH:mm:ss} - Layout service ready\\n" +
                $"?? {DateTime.Now.AddSeconds(-20):HH:mm:ss} - Window factories registered\\n" +
                $"?? {DateTime.Now.AddSeconds(-15):HH:mm:ss} - Main window created\\n" +
                $"?? {DateTime.Now.AddSeconds(-10):HH:mm:ss} - Default layout applied\\n" +
                $"?? {DateTime.Now.AddSeconds(-5):HH:mm:ss} - Application ready\\n\\n" +
                "? All systems operational\\n" +
                "?? Memory usage: 45.2 MB\\n" +
                "? Startup time: 1.2 seconds\\n\\n" +
                "?? Category: Debug\\n" +
                "?? Default: Bottom Panel"
            );
        }
    }

    public class DynamicContentFactory : WindowFactoryBase
    {
        public override string WindowId => "dynamic-content";
        public override string DisplayName => "Dynamic Content";
        public override string Category => "Sample";
        public override DockingArea DefaultDockingArea => DockingArea.Center;
        public override bool AllowMultipleInstances => true;

        public override DockableWindow CreateWindow()
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            return CreateBasicWindow(
                $"Dynamic {timestamp}",
                $"? Dynamic Content Window\\n\\n" +
                $"?? Created at: {timestamp}\\n" +
                $"?? Unique ID: {Guid.NewGuid().ToString("N")[..8]}\\n\\n" +
                $"? This demonstrates:\\n" +
                $"• Multiple instances allowed\\n" +
                $"• Runtime window creation\\n" +
                $"• Automatic ID generation\\n" +
                $"• Framework integration\\n\\n" +
                $"?? Each instance is independent\\n" +
                $"?? Can create unlimited instances\\n" +
                $"?? Framework handles all complexity\\n\\n" +
                $"?? Category: Sample\\n" +
                $"?? Default: Center Panel"
            );
        }
    }
}