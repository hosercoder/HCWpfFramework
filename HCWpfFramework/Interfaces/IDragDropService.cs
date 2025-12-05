using System.Windows;
using HCWpfFramework.Models;

namespace HCWpfFramework.Interfaces
{
    /// <summary>
    /// Service for handling drag and drop operations in the docking system
    /// Users don't need to implement this - framework handles all complexity
    /// </summary>
    public interface IDragDropService
    {
        /// <summary>
        /// Start a drag operation for a dockable window
        /// </summary>
        /// <param name="window">The window being dragged</param>
        /// <param name="startPoint">The starting point of the drag</param>
        void StartDrag(DockableWindow window, Point startPoint);
        
        /// <summary>
        /// Handle drag over a potential drop target
        /// </summary>
        /// <param name="dropTarget">The potential drop target</param>
        /// <param name="currentPoint">Current mouse position</param>
        /// <returns>True if drop is allowed</returns>
        bool HandleDragOver(FrameworkElement dropTarget, Point currentPoint);
        
        /// <summary>
        /// Complete a drop operation
        /// </summary>
        /// <param name="dropTarget">The drop target</param>
        /// <param name="dropPoint">The drop point</param>
        /// <returns>True if drop was successful</returns>
        bool CompleteDrop(FrameworkElement dropTarget, Point dropPoint);
        
        /// <summary>
        /// Cancel current drag operation
        /// </summary>
        void CancelDrag();
        
        /// <summary>
        /// Check if a drag operation is currently in progress
        /// </summary>
        bool IsDragging { get; }
        
        /// <summary>
        /// Get the window currently being dragged
        /// </summary>
        DockableWindow? DraggedWindow { get; }
        
        /// <summary>
        /// Event fired when drag starts
        /// </summary>
        event EventHandler<DragStartedEventArgs> DragStarted;
        
        /// <summary>
        /// Event fired when drag completes (success or cancel)
        /// </summary>
        event EventHandler<DragCompletedEventArgs> DragCompleted;
    }

    /// <summary>
    /// Event args for drag started
    /// </summary>
    public class DragStartedEventArgs : EventArgs
    {
        public DockableWindow Window { get; }
        public Point StartPoint { get; }
        
        public DragStartedEventArgs(DockableWindow window, Point startPoint)
        {
            Window = window;
            StartPoint = startPoint;
        }
    }

    /// <summary>
    /// Event args for drag completed
    /// </summary>
    public class DragCompletedEventArgs : EventArgs
    {
        public DockableWindow Window { get; }
        public bool Success { get; }
        public FrameworkElement? DropTarget { get; }
        
        public DragCompletedEventArgs(DockableWindow window, bool success, FrameworkElement? dropTarget = null)
        {
            Window = window;
            Success = success;
            DropTarget = dropTarget;
        }
    }
}