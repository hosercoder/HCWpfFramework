using System.Windows;
using System.Windows.Controls;
using HCWpfFramework.Models;

namespace HCWpfFramework.Controls
{
    public partial class DockableContent : UserControl
    {
        public static readonly DependencyProperty WindowDataProperty =
            DependencyProperty.Register(nameof(WindowData), typeof(DockableWindow), typeof(DockableContent));

        public DockableWindow? WindowData
        {
            get => (DockableWindow?)GetValue(WindowDataProperty);
            set => SetValue(WindowDataProperty, value);
        }

        public event EventHandler<DockableWindow>? WindowFloatRequested;
        public event EventHandler<DockableWindow>? WindowCloseRequested;

        public DockableContent()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void FloatButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowData != null)
            {
                WindowFloatRequested?.Invoke(this, WindowData);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowData != null)
            {
                WindowCloseRequested?.Invoke(this, WindowData);
            }
        }
    }
}