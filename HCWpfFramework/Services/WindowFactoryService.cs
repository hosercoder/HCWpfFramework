using System.Reflection;
using HCWpfFramework.Interfaces;
using HCWpfFramework.Models;

namespace HCWpfFramework.Services
{
    /// <summary>
    /// Service that manages window factories and provides easy window creation
    /// This handles all the complexity - users just implement IWindowFactory
    /// </summary>
    public class WindowFactoryService : IWindowFactoryService
    {
        private readonly Dictionary<string, IWindowFactory> _factories = new();
        private readonly Dictionary<string, List<IWindowFactory>> _categorizedFactories = new();

        public event EventHandler<WindowFactoryRegisteredEventArgs>? FactoryRegistered;

        public void RegisterFactory(IWindowFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factories[factory.WindowId] = factory;

            // Organize by category
            if (!_categorizedFactories.ContainsKey(factory.Category))
            {
                _categorizedFactories[factory.Category] = new List<IWindowFactory>();
            }
            _categorizedFactories[factory.Category].Add(factory);

            FactoryRegistered?.Invoke(this, new WindowFactoryRegisteredEventArgs(factory));
        }

        public void RegisterFactoriesFromAssembly(Assembly assembly)
        {
            try
            {
                var factoryTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IWindowFactory).IsAssignableFrom(t))
                    .Where(t => t.GetConstructor(Type.EmptyTypes) != null); // Must have parameterless constructor

                foreach (var factoryType in factoryTypes)
                {
                    try
                    {
                        var factory = (IWindowFactory)Activator.CreateInstance(factoryType)!;
                        RegisterFactory(factory);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to create factory instance for {factoryType.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to scan assembly {assembly.FullName} for window factories: {ex.Message}");
            }
        }

        public DockableWindow? CreateWindow(string windowId)
        {
            if (_factories.TryGetValue(windowId, out var factory))
            {
                try
                {
                    var window = factory.CreateWindow();
                    
                    // Ensure the window has the correct ID
                    if (string.IsNullOrEmpty(window.Id))
                    {
                        window.Id = factory.AllowMultipleInstances 
                            ? $"{windowId}_{Guid.NewGuid():N}"
                            : windowId;
                    }
                    
                    return window;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to create window from factory {windowId}: {ex.Message}");
                    return null;
                }
            }

            return null;
        }

        public IEnumerable<IWindowFactory> GetAllFactories()
        {
            return _factories.Values;
        }

        public IEnumerable<IWindowFactory> GetFactoriesByCategory(string category)
        {
            return _categorizedFactories.TryGetValue(category, out var factories) 
                ? factories 
                : Enumerable.Empty<IWindowFactory>();
        }

        /// <summary>
        /// Create multiple windows for a category (useful for default layouts)
        /// </summary>
        /// <param name="category">Category name</param>
        /// <returns>Collection of created windows</returns>
        public IEnumerable<DockableWindow> CreateWindowsForCategory(string category)
        {
            var factories = GetFactoriesByCategory(category);
            foreach (var factory in factories)
            {
                var window = CreateWindow(factory.WindowId);
                if (window != null)
                {
                    yield return window;
                }
            }
        }

        /// <summary>
        /// Get available window categories
        /// </summary>
        /// <returns>Collection of category names</returns>
        public IEnumerable<string> GetCategories()
        {
            return _categorizedFactories.Keys;
        }
    }
}