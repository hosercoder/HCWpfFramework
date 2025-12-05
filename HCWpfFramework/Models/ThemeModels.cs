namespace HCWpfFramework.Models
{
    public enum ThemeType
    {
        Light,
        Dark,
        System // Follows system theme
    }

    public class Theme
    {
        public string Name { get; set; } = string.Empty;
        public ThemeType Type { get; set; }
        public Dictionary<string, object> Resources { get; set; } = new();
    }

    public class ThemeChangedEventArgs : EventArgs
    {
        public Theme NewTheme { get; }
        public Theme OldTheme { get; }

        public ThemeChangedEventArgs(Theme newTheme, Theme oldTheme)
        {
            NewTheme = newTheme;
            OldTheme = oldTheme;
        }
    }
}