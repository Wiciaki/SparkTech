namespace SparkTech.SDK.GUI
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Rendering;

    internal static class Clock
    {
        static Clock()
        {
            Render.OnEndScene += OnEndScene;
        }

        public static bool Enabled;

        private static Size2 size;

        private static Point point;

        private static string Text()
        {
            return DateTime.Now.ToShortTimeString();
        }

        public static void UpdateSize()
        {
            size = Theme.MeasureText(Text() + "0");

            point = new Point((Render.Resolution().Width - size.Width) / 2, 0);
        }

        private static void OnEndScene()
        {
            if (Enabled)
            {
                Theme.DrawTextBox(point, size, Text());
            }
        }
    }
}