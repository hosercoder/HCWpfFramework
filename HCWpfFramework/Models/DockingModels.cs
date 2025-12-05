namespace HCWpfFramework.Models
{
    public enum LayoutType
    {
        Single = 1,
        TopBottom = 2,
        ThreePane = 3,
        FourPane = 4
    }

    public class DockableWindow
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public object? Content { get; set; }
        public bool CanClose { get; set; } = true;
        public bool CanFloat { get; set; } = true;
        public bool IsFloating { get; set; }
    }

    public class LayoutChangedEventArgs : EventArgs
    {
        public LayoutType NewLayout { get; }
        public LayoutType OldLayout { get; }

        public LayoutChangedEventArgs(LayoutType newLayout, LayoutType oldLayout)
        {
            NewLayout = newLayout;
            OldLayout = oldLayout;
        }
    }
}