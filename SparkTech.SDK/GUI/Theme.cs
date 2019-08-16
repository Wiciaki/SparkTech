namespace SparkTech.SDK.GUI
{
    using SharpDX;

    using Size = System.Drawing.Size;
    using Point = System.Drawing.Point;

    public static class Theme
    {
        private static ITheme theme;

        internal static void SetTheme(ITheme t)
        {
            theme = t;

            Clock.UpdateSize();
            Menu.Menu.UpdateAllSizes();
        }

        public static int ItemGroupDistance => theme.ItemGroupDistance;

        public static Color BackgroundColor => theme.BackgroundColor;

        public static Size MeasureText(string text) => theme.MeasureText(text);

        public static void DrawTextBox(Point point, Size size, string text, Color? color = null) => theme.DrawTextBox(point, size, text, color);

        public static void DrawBox(Point point, Size size, Color color) => theme.DrawBox(point, size, color);
        
        public static void DrawBorders(Point point, params Size[] sizes) => theme.DrawBorders(point, sizes);
    }
}