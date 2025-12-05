using System.Collections.ObjectModel;
using HCWpfFramework.Models;

namespace HCWpfFramework.Interfaces
{
    /// <summary>
    /// Interface for creating application-specific dockable windows
    /// Users implement this to define their custom windows
    /// </summary>
    public interface IWindowFactory
    {
        /// <summary>
        /// Gets the unique identifier for this window type
        /// </summary>
        string WindowId { get; }
        
        /// <summary>
        /// Gets the display name for this window type
        /// </summary>
        string DisplayName { get; }
        
        /// <summary>
        /// Gets the default docking area for this window type
        /// </summary>
        DockingArea DefaultDockingArea { get; }
        
        /// <summary>
        /// Creates a new instance of the dockable window
        /// </summary>
        /// <returns>A configured DockableWindow instance</returns>
        DockableWindow CreateWindow();
        
        /// <summary>
        /// Determines if this window type can have multiple instances
        /// </summary>
        bool AllowMultipleInstances { get; }
        
        /// <summary>
        /// Gets the category for organizing windows in menus
        /// </summary>
        string Category { get; }
    }

    /// <summary>
    /// Service for managing window factories and creation
    /// </summary>
    public interface IWindowFactoryService
    {
        /// <summary>
        /// Register a window factory
        /// </summary>
        /// <param name="factory">The window factory to register</param>
        void RegisterFactory(IWindowFactory factory);
        
        /// <summary>
        /// Register multiple factories from an assembly
        /// </summary>
        /// <param name="assembly">Assembly to scan for window factories</param>
        void RegisterFactoriesFromAssembly(System.Reflection.Assembly assembly);
        
        /// <summary>
        /// Create a window by its ID
        /// </summary>
        /// <param name="windowId">The window ID</param>
        /// <returns>The created window or null if not found</returns>
        DockableWindow? CreateWindow(string windowId);
        
        /// <summary>
        /// Get all registered factories
        /// </summary>
        /// <returns>Collection of registered factories</returns>
        IEnumerable<IWindowFactory> GetAllFactories();
        
        /// <summary>
        /// Get factories by category
        /// </summary>
        /// <param name="category">The category name</param>
        /// <returns>Factories in the specified category</returns>
        IEnumerable<IWindowFactory> GetFactoriesByCategory(string category);
        
        /// <summary>
        /// Event fired when a new factory is registered
        /// </summary>
        event EventHandler<WindowFactoryRegisteredEventArgs> FactoryRegistered;
    }

    /// <summary>
    /// Event args for factory registration
    /// </summary>
    public class WindowFactoryRegisteredEventArgs : EventArgs
    {
        public IWindowFactory Factory { get; }
        
        public WindowFactoryRegisteredEventArgs(IWindowFactory factory)
        {
            Factory = factory;
        }
    }
}