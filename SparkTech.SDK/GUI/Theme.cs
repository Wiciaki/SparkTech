namespace SparkTech.SDK.GUI
{
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    using Size = System.Drawing.Size;
    using Point = System.Drawing.Point;

    public static class Theme
    {
        private static ITheme theme;

        internal static void SetTheme(ITheme t)
        {
            theme = t;

            var roots = Menu.Menu.GetRootMenus();

            foreach (var item in roots.Concat(roots.SelectMany(menu => menu.GetDescensants())))
            {
                item.UpdateSize();
            }
        }

        public static int ItemGroupDistance => theme.ItemGroupDistance;

        public static Color BackgroundColor => theme.BackgroundColor;

        public static Size MeasureText(string text) => theme.MeasureText(text);

        public static void DrawTextBox(string text, Point point, Size size) => theme.DrawTextBox(text, point, size);

        public static void DrawBox(Color color, Point point, Size size) => theme.DrawBox(color, point, size);
        
        public static void DrawBorders(IEnumerable<Size> sizes, Point point) => theme.DrawBorders(sizes, point);
    }
}