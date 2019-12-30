namespace Surgical.SDK.GUI
{
    using System;

    using SharpDX;

    using Surgical.SDK.Rendering;

    internal static class Clock
    {
        static Clock()
        {
            Render.OnEndScene += OnEndScene;
        }

        private static int mode;

        internal static void SetMode(int value)
        {
            mode = value;
        }

        private static Size2 size;

        private static Point point;

        private static string GetText(DateTime dateTime)
        {
            return Platform.Name + " - " + dateTime.ToLongTimeString();
        }

        internal static void UpdateSize()
        {
            size = Theme.MeasureText(GetText(DateTime.Today));

            point = new Point((Render.Resolution().Width - size.Width) / 2, 0);
        }

        private static void OnEndScene()
        {
            if (mode == 2)
            {
                return;
            }

            var date = DateTime.Now;

            if (mode == 0 || mode == 1 && date.Second <= 5)
            {
                Theme.DrawTextBox(point, size, GetText(date), true);
            }
        }
    }
}