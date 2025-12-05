using System.Collections.ObjectModel;
using HCWpfFramework.Models;

namespace HCWpfFramework.Interfaces
{
    /// <summary>
    /// Service for managing application layouts and default dockable windows
    /// </summary>
    public interface ILayoutService
    {
        /// <summary>
        /// Gets the current layout type
        /// </summary>
        LayoutType CurrentLayout { get; }

        /// <summary>
        /// Gets all available layouts
        /// </summary>
        IEnumerable<LayoutType> AvailableLayouts { get; }

        /// <summary>
        /// Event fired when layout changes
        /// </summary>
        event EventHandler<LayoutChangedEventArgs>? LayoutChanged;

        /// <summary>
        /// Sets the current layout
        /// </summary>
        /// <param name="layoutType">The layout type to apply</param>
        void SetLayout(LayoutType layoutType);

        /// <summary>
        /// Creates default dockable windows for a specific layout area
        /// </summary>
        /// <param name="area">The docking area (Left, Right, Center, Bottom)</param>
        /// <returns>Collection of default dockable windows</returns>
        ObservableCollection<DockableWindow> CreateDefaultWindows(DockingArea area);

        /// <summary>
        /// Creates a complete default layout with all panels populated
        /// </summary>
        /// <returns>Default layout configuration</returns>
        DefaultLayout CreateDefaultLayout();

        /// <summary>
        /// Registers a custom window factory for creating application-specific windows
        /// </summary>
        /// <param name="area">The docking area</param>
        /// <param name="factory">Factory function to create windows</param>
        void RegisterWindowFactory(DockingArea area, Func<IEnumerable<DockableWindow>> factory);

        /// <summary>
        /// Saves the current layout configuration
        /// </summary>
        void SaveLayoutConfiguration();

        /// <summary>
        /// Loads the saved layout configuration
        /// </summary>
        void LoadLayoutConfiguration();

        /// <summary>
        /// Resets to default layout
        /// </summary>
        void ResetToDefault();
    }

    /// <summary>
    /// Represents different docking areas
    /// </summary>
    public enum DockingArea
    {
        Left,
        Right,
        Center,
        Bottom,
        Top
    }

    /// <summary>
    /// Represents a complete layout configuration
    /// </summary>
    public class DefaultLayout
    {
        public ObservableCollection<DockableWindow> LeftPanelWindows { get; set; } = new();
        public ObservableCollection<DockableWindow> RightPanelWindows { get; set; } = new();
        public ObservableCollection<DockableWindow> CenterPanelWindows { get; set; } = new();
        public ObservableCollection<DockableWindow> BottomPanelWindows { get; set; } = new();
        public ObservableCollection<DockableWindow> TopPanelWindows { get; set; } = new();
        public LayoutType LayoutType { get; set; } = LayoutType.ThreePane;
    }
}