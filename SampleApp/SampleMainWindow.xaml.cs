using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using HCWpfFramework.Models;
using HCWpfFramework.ViewModels;
using HCWpfFramework.Views;
using SampleApp.ViewModels;

namespace SampleApp
{
    /// <summary>
    /// Sample MainWindow that demonstrates complete framework inheritance
    /// Inherits from FrameworkMainWindow for maximum framework integration with minimal code
    /// </summary>
    public class SampleMainWindow : FrameworkMainWindow
    {
        /// <summary>
        /// Default constructor - framework handles initialization
        /// </summary>
        public SampleMainWindow()
        {
            // Framework base class handles all initialization
        }

        /// <summary>
        /// Constructor with dependency injection
        /// </summary>
        /// <param name="viewModel">The SampleMainViewModel to bind</param>
        public SampleMainWindow(SampleMainViewModel viewModel) : base(viewModel)
        {
            // Framework base class handles ViewModel binding and lifecycle
        }

        /// <summary>
        /// Override to provide ViewModel when using default constructor
        /// </summary>
        protected override MainViewModelBase? CreateViewModel(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<SampleMainViewModel>();
        }

        /// <summary>
        /// Override to customize window after ViewModel is set
        /// </summary>
        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            
            // Minimal customization - framework handles everything else
            Title = "HCWpfFramework Sample - Complete Inheritance Demo";
            Height = 750;
            Width = 1200;
        }
    }
}