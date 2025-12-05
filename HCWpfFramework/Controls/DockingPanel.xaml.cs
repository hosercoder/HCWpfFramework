using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HCWpfFramework.Models;

namespace HCWpfFramework.Controls
{
    public partial class DockingPanel : UserControl
    {
        public static readonly DependencyProperty PanelTitleProperty =
            DependencyProperty.Register(nameof(PanelTitle), typeof(string), typeof(DockingPanel), new PropertyMetadata("Docking Panel"));

        public static readonly DependencyProperty DockableWindowsProperty =
            DependencyProperty.Register(nameof(DockableWindows), typeof(ObservableCollection<DockableWindow>), typeof(DockingPanel));

        public string PanelTitle
        {
            get => (string)GetValue(PanelTitleProperty);
            set => SetValue(PanelTitleProperty, value);
        }

        public ObservableCollection<DockableWindow> DockableWindows
        {
            get => (ObservableCollection<DockableWindow>)GetValue(DockableWindowsProperty);
            set => SetValue(DockableWindowsProperty, value);
        }

        public event EventHandler<DockableWindow>? WindowFloatRequested;
        public event EventHandler<DockableWindow>? WindowCloseRequested;
        public event EventHandler<DockableWindow>? WindowDropped;

        /// <summary>
        /// Gets a value indicating whether this panel is currently in drag mode
        /// </summary>
        public bool IsDragging => _isDragging;

        private bool _isDragging = false;
        private DockableWindow? _draggedWindow = null;
        private System.Windows.Threading.DispatcherTimer? _dragResetTimer;
        private bool _isDropZoneVisible = false;
        private DockableWindow? _currentDraggedWindow = null;
        private System.Windows.Threading.DispatcherTimer? _dropZoneDelayTimer;

        public DockingPanel()
        {
            InitializeComponent();
            DataContext = this;

            if (DockableWindows == null)
            {
                DockableWindows = new ObservableCollection<DockableWindow>();
            }

            // Add event handlers to detect when mouse is released to reset drag state
            this.MouseUp += DockingPanel_MouseUp;
            this.MouseLeave += DockingPanel_MouseLeave;
            this.KeyDown += DockingPanel_KeyDown;

            // Initialize drag reset timer as a failsafe
            InitializeDragResetTimer();
            
            // Initialize drop zone delay timer to prevent flashing
            InitializeDropZoneDelayTimer();
        }

        private void DockingPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Reset drag state when mouse is released
            if (_isDragging)
            {
                ResetDragState();
            }
        }

        private void DockingPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            // Only reset drag state if we're not currently in a drag operation
            // and the mouse has actually left the control bounds
            if (!_isDragging)
            {
                var position = e.GetPosition(this);
                var bounds = new Rect(0, 0, ActualWidth, ActualHeight);
                
                if (!bounds.Contains(position))
                {
                    HideDropZone();
                }
            }
        }

        private void DockingPanel_KeyDown(object sender, KeyEventArgs e)
        {
            // Reset drag state when Escape key is pressed
            if (e.Key == Key.Escape)
            {
                ResetDragState();
                e.Handled = true;
            }
        }

        private void DockableContent_WindowFloatRequested(object sender, DockableWindow e)
        {
            WindowFloatRequested?.Invoke(this, e);
        }

        private void DockableContent_WindowCloseRequested(object sender, DockableWindow e)
        {
            WindowCloseRequested?.Invoke(this, e);
        }

        private void TabCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is DockableWindow window)
            {
                WindowCloseRequested?.Invoke(this, window);
            }
        }

        private void TabItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
            {
                if (sender is TabItem tabItem && tabItem.DataContext is DockableWindow window)
                {
                    _isDragging = true;
                    _draggedWindow = window;
                    
                    try
                    {
                        var dragData = new DataObject("DockableWindow", window);
                        var result = DragDrop.DoDragDrop(tabItem, dragData, DragDropEffects.Move);
                        
                        // Handle the result of the drag operation
                        if (result == DragDropEffects.None)
                        {
                            // Drag was cancelled, ensure UI state is reset
                            ResetDragState();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Drag operation failed: {ex.Message}");
                        ResetDragState();
                    }
                    finally
                    {
                        _isDragging = false;
                        _draggedWindow = null;
                    }
                }
            }
        }

        private void TabControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("DockableWindow"))
            {
                var draggedWindow = (DockableWindow)e.Data.GetData("DockableWindow");
                if (draggedWindow != null)
                {
                    // Store the currently dragged window
                    _currentDraggedWindow = draggedWindow;
                    
                    // Always allow dropping - the window can be moved between panels
                    e.Effects = DragDropEffects.Move;
                    
                    // Use delayed showing to prevent flashing
                    if (!_isDropZoneVisible)
                    {
                        _dropZoneDelayTimer?.Stop();
                        _dropZoneDelayTimer?.Start();
                    }
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                    HideDropZone();
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
                HideDropZone();
            }
            e.Handled = true;
        }

        private void TabControl_DragLeave(object sender, DragEventArgs e)
        {
            // Get the position relative to the TabControl
            var position = e.GetPosition(WindowTabControl);
            var bounds = new Rect(-10, -10, WindowTabControl.ActualWidth + 20, WindowTabControl.ActualHeight + 20);
            
            // Only hide drop zone if we're actually leaving the control (with some tolerance)
            if (!bounds.Contains(position))
            {
                HideDropZone();
            }
            e.Handled = true;
        }

        private void TabControl_Drop(object sender, DragEventArgs e)
        {
            try
            {
                HideDropZone();
                
                if (e.Data.GetDataPresent("DockableWindow"))
                {
                    var window = (DockableWindow)e.Data.GetData("DockableWindow");
                    if (window != null)
                    {
                        WindowDropped?.Invoke(this, window);
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Drop operation failed: {ex.Message}");
                e.Effects = DragDropEffects.None;
            }
            finally
            {
                ResetDragState();
            }
            e.Handled = true;
        }

        private void InitializeDragResetTimer()
        {
            _dragResetTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // Reset after 5 seconds of inactivity (failsafe only)
            };
            _dragResetTimer.Tick += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"[{PanelTitle}] Timer-based drag state reset");
                ResetDragState();
                _dragResetTimer?.Stop();
            };
        }

        private void InitializeDropZoneDelayTimer()
        {
            _dropZoneDelayTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100) // Small delay to prevent flashing
            };
            _dropZoneDelayTimer.Tick += (s, e) =>
            {
                _dropZoneDelayTimer?.Stop();
                if (!_isDropZoneVisible && _currentDraggedWindow != null)
                {
                    _isDropZoneVisible = true;
                    DropZoneIndicator.Visibility = Visibility.Visible;
                    
                    // Start the failsafe timer
                    if (!_dragResetTimer?.IsEnabled ?? false)
                    {
                        _dragResetTimer?.Start();
                    }
                }
            };
        }

        private void HideDropZone()
        {
            // Stop the delay timer to prevent showing
            _dropZoneDelayTimer?.Stop();
            
            if (_isDropZoneVisible)
            {
                _isDropZoneVisible = false;
                DropZoneIndicator.Visibility = Visibility.Collapsed;
                
                // Stop the timer since we're intentionally hiding the drop zone
                _dragResetTimer?.Stop();
            }
        }

        public void ResetDragState()
        {
            // Stop all timers
            _dragResetTimer?.Stop();
            _dropZoneDelayTimer?.Stop();
            
            // Debug information
            System.Diagnostics.Debug.WriteLine($"[{PanelTitle}] Resetting drag state - Was dragging: {_isDragging}");
            
            // Ensure all drag-related UI elements are reset
            HideDropZone();
            _isDragging = false;
            _draggedWindow = null;
            _currentDraggedWindow = null;
        }

        public void AddWindow(DockableWindow window)
        {
            if (!DockableWindows.Contains(window))
            {
                DockableWindows.Add(window);
                WindowTabControl.SelectedItem = window;
            }
        }

        public void RemoveWindow(DockableWindow window)
        {
            DockableWindows.Remove(window);
        }

        public void ClearWindows()
        {
            DockableWindows.Clear();
        }
    }
}