using HCWpfFramework.Interfaces;
using HCWpfFramework.Models;

namespace HCWpfFramework.ViewModels
{
    /// <summary>
    /// Base class for window factories that provides common functionality
    /// Users can inherit from this for easier implementation
    /// </summary>
    public abstract class WindowFactoryBase : IWindowFactory
    {
        public abstract string WindowId { get; }
        public abstract string DisplayName { get; }
        public virtual DockingArea DefaultDockingArea => DockingArea.Center;
        public virtual bool AllowMultipleInstances => true;
        public virtual string Category => "General";

        public abstract DockableWindow CreateWindow();

        /// <summary>
        /// Helper method to create a basic DockableWindow with common settings
        /// </summary>
        /// <param name="title">Window title</param>
        /// <param name="description">Window description/content</param>
        /// <param name="canClose">Whether window can be closed</param>
        /// <param name="canFloat">Whether window can float</param>
        /// <returns>Configured DockableWindow</returns>
        protected DockableWindow CreateBasicWindow(string title, string description, bool canClose = true, bool canFloat = true)
        {
            return new DockableWindow
            {
                Id = AllowMultipleInstances ? $"{WindowId}_{Guid.NewGuid():N}" : WindowId,
                Title = title,
                Description = description,
                CanClose = canClose,
                CanFloat = canFloat
            };
        }

        /// <summary>
        /// Helper method to create a window with custom content
        /// </summary>
        /// <param name="title">Window title</param>
        /// <param name="content">Custom content object</param>
        /// <param name="canClose">Whether window can be closed</param>
        /// <param name="canFloat">Whether window can float</param>
        /// <returns>Configured DockableWindow</returns>
        protected DockableWindow CreateContentWindow(string title, object content, bool canClose = true, bool canFloat = true)
        {
            return new DockableWindow
            {
                Id = AllowMultipleInstances ? $"{WindowId}_{Guid.NewGuid():N}" : WindowId,
                Title = title,
                Content = content,
                CanClose = canClose,
                CanFloat = canFloat
            };
        }
    }

    /// <summary>
    /// Generic window factory for simple text-based windows
    /// </summary>
    public class TextWindowFactory : WindowFactoryBase
    {
        private readonly string _windowId;
        private readonly string _displayName;
        private readonly string _category;
        private readonly Func<string> _contentProvider;
        private readonly DockingArea _defaultArea;

        public override string WindowId => _windowId;
        public override string DisplayName => _displayName;
        public override string Category => _category;
        public override DockingArea DefaultDockingArea => _defaultArea;

        public TextWindowFactory(string windowId, string displayName, Func<string> contentProvider, 
            string category = "General", DockingArea defaultArea = DockingArea.Center)
        {
            _windowId = windowId;
            _displayName = displayName;
            _contentProvider = contentProvider;
            _category = category;
            _defaultArea = defaultArea;
        }

        public override DockableWindow CreateWindow()
        {
            var content = _contentProvider();
            return CreateBasicWindow(DisplayName, content);
        }
    }

    /// <summary>
    /// Window factory for UserControl-based windows
    /// </summary>
    /// <typeparam name="T">The UserControl type</typeparam>
    public class UserControlWindowFactory<T> : WindowFactoryBase where T : System.Windows.Controls.UserControl, new()
    {
        private readonly string _windowId;
        private readonly string _displayName;
        private readonly string _category;
        private readonly DockingArea _defaultArea;

        public override string WindowId => _windowId;
        public override string DisplayName => _displayName;
        public override string Category => _category;
        public override DockingArea DefaultDockingArea => _defaultArea;

        public UserControlWindowFactory(string windowId, string displayName, 
            string category = "General", DockingArea defaultArea = DockingArea.Center)
        {
            _windowId = windowId;
            _displayName = displayName;
            _category = category;
            _defaultArea = defaultArea;
        }

        public override DockableWindow CreateWindow()
        {
            var userControl = new T();
            return CreateContentWindow(DisplayName, userControl);
        }
    }
}