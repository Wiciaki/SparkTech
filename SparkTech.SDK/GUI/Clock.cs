namespace SparkTech.SDK.GUI
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Rendering;

    public static class Clock
    {
        private static int mode;

        private static int elements;

        private static bool useBackground;

        private static Color? customColor;

        private static Size2 size;

        private static Point point;

        internal static void SetMode(int value)
        {
            mode = value;
        }

        internal static void SetBackground(bool b)
        {
            useBackground = b;
        }

        internal static void SetElements(int i)
        {
            elements = i;

            if (Platform.HasRenderAPI)
            {
                UpdateSize(); 
            }
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

        static Clock()
        {
            if (Platform.HasRenderAPI)
            {
                Render.OnEndScene += OnEndScene;
            }
        }

        private static string GetText(DateTime dateTime)
        {
            var text = string.Empty;

            if (elements == 0 || elements == 1)
            {
                text += Platform.Name;
            }

            if (elements == 0)
            {
                text += " - ";
            }

            if (elements == 0 || elements == 2)
            {
                text += dateTime.ToLongTimeString();
            }

            return text;
        }

        internal static void UpdateSize()
        {
            size = Theme.MeasureText(GetText(DateTime.Today));
            point = new Point((Render.Resolution.Width - size.Width) / 2, Theme.WatermarkOffset);
        }

        private static void OnEndScene()
        {
            var date = DateTime.Now;

            switch (mode)
            {
                case 1:
                    if (!Menu.Menu.IsOpen)
                    {
                        return;
                    }
                    break;
                case 2:
                    if (date.Second > 5)
                    {
                        return;
                    }
                    break;
                case 3:
                    return;
            }

            var bgcolor = useBackground ? Theme.BackgroundColor : Color.Transparent;
            var txtcolor = customColor ?? Theme.TextColor;

            Theme.DrawTextBox(point, size, bgcolor, txtcolor, GetText(date), true);
        }
    }
}