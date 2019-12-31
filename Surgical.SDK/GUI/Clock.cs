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

        private static bool useBackground;

        private static Color? customColor;

        internal static void SetMode(int value)
        {
            mode = value;
        }

        internal static void SetBackground(bool b)
        {
            useBackground = b;
        }

        internal static void SetCustomColor(Color color, bool b)
        {
            if (!b)
            {
                customColor = null;
            }
            else
            {
                customColor = color;
            }
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
            if (mode >= 3)
            {
                return;
            }

            var date = DateTime.Now;

            var b = mode switch
            {
                0 => true,
                1 => Menu.Menu.IsOpen,
                2 => date.Second <= 5,
                _ => throw new IndexOutOfRangeException()
            };

            if (!b)
            {
                return;
            }

            var bgcolor = useBackground ? Theme.BackgroundColor : Color.Transparent;
            var txtcolor = customColor ?? Theme.TextColor;

            Theme.DrawTextBox(point, size, bgcolor, txtcolor, GetText(date), true);
        }
    }
}