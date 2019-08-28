namespace SparkTech.SDK.GUI
{
    using SharpDX;

    public class Theme
    {
        private static ITheme theme;

        internal static void SetTheme(ITheme t)
        {
            theme?.Dispose();

            theme = t;

            Clock.UpdateSize();

            Menu.Menu.UpdateAllSizes();
            Notifications.Notification.UpdateAllSizes();
        }

        public static int MinItemHeight => theme.MinItemHeight;

        public static Color BackgroundColor => theme.BackgroundColor;

        public static Size2 MeasureText(string text)
        {
            return theme.MeasureText(text);
        }

        public static void DrawTextBox(Point point, Size2 size, string text, bool forceCentered = false, Color? color = null)
        {
            theme.DrawTextBox(point, size, text, forceCentered, color);
        }

        public static void DrawBox(Point point, Size2 size, Color color)
        {
            theme.DrawBox(point, size, color);
        }

        public static void DrawBorders(Point point, params Size2[] sizes)
        {
            theme.DrawBorders(point, sizes);
        }
    }
}