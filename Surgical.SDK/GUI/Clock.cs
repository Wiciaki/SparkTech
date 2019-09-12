namespace Surgical.SDK.GUI
{
    using System;

    using SharpDX;

    using Surgical.SDK.API;
    using Surgical.SDK.Rendering;

    internal static class Clock
    {
        static Clock()
        {
            Render.OnEndScene += OnEndScene;
        }

        private static int mode;

        internal static void SetMode(int m)
        {
            mode = m;
        }

        private static Size2 size;

        private static Point point;

        private static string Text()
        {
            return Platform.PlatformName + " - " + DateTime.Now.ToLongTimeString();
        }

        internal static void UpdateSize()
        {
            size = Theme.MeasureText(Text() + "0");

            point = new Point((Render.Resolution().Width - size.Width) / 2, 0);
        }

        private static void OnEndScene()
        {
            if (mode == 0)
            {
                Theme.DrawTextBox(point, size, Text(), true, Color.Transparent);
            }
        }
    }
}