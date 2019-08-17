namespace SparkTech.SDK.GUI
{
    using SharpDX;

    public static class Theme
    {
        private static ITheme theme;

        internal static void SetTheme(ITheme t)
        {
            theme = t;

            Clock.UpdateSize();
            Notifications.Notification.UpdateAllSizes();
            Menu.Menu.UpdateAllSizes();
        }

        public static int ItemGroupDistance => theme.ItemGroupDistance;

        public static Color BackgroundColor => theme.BackgroundColor;

        public static Size2 MeasureText(string text) => theme.MeasureText(text);

        public static void DrawTextBox(Point point, Size2 size, string text, Color? color = null) => theme.DrawTextBox(point, size, text, color);

        public static void DrawBox(Point point, Size2 size, Color color) => theme.DrawBox(point, size, color);
        
        public static void DrawBorders(Point point, params Size2[] sizes) => theme.DrawBorders(point, sizes);
    }
}