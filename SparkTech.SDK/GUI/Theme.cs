namespace SparkTech.SDK.GUI
{
    using System;

    using SharpDX;
    using SparkTech.SDK.GUI.Themes;

    public static class Theme
    {
        public static int WatermarkOffset { get; internal set; }

        private static readonly ITheme PlatformTheme;

        private static ITheme theme;

        static Theme()
        {
            if (!Platform.HasRenderAPI)
            {
                theme = new NullTheme();
                return;
            }

            PlatformTheme = Platform.PlatformTheme;
            theme = PlatformTheme ?? DefaultTheme.Create();
            theme.Start();

            Menu.Menu.UpdateDecalSizes();
            Clock.UpdateSize();
        }

        internal static void SetTheme(int i)
        {
            theme.Pause();
            theme = GetThemeByIndex(i);
            theme.Start();

            Menu.Menu.UpdateDecalSizes();
            Clock.UpdateSize();

            Menu.Menu.UpdateAllSizes();
            Notifications.Notification.UpdateAllSizes();
        }

        private static ITheme GetThemeByIndex(int i)
        {
            switch (i)
            {
                case 0: return PlatformTheme ?? throw new InvalidOperationException();
                case 1: return DefaultTheme.Create();
                case 2: return AlternateBordersTheme.Create();
                case 3: return SimpleTheme.CreateBlack();
                case 4: return SimpleTheme.CreateWhite();
                case 5: return SimpleTheme.CreateBlue();
                case 6: return StylishTheme.CreateRed();
                case 7: return StylishTheme.CreateBlue();
                default: throw new IndexOutOfRangeException();
            }
        }

        #region API Proxy

        public static int MinItemHeight => theme.MinItemHeight;

        public static Color BackgroundColor => theme.BackgroundColor;

        public static Color BorderColor => theme.BorderColor;

        public static Color TextColor => theme.TextColor;

        public static Size2 MeasureText(string text)
        {
            return theme.MeasureText(text);
        }

        public static void DrawTextBox(Point point, Size2 size, string text, bool forceCentered = false)
        {
            DrawTextBox(point, size, BackgroundColor, text, forceCentered);
        }

        public static void DrawTextBox(Point point, Size2 size, Color bgcolor, string text, bool forceCentered = false)
        {
            DrawTextBox(point, size, bgcolor, TextColor, text, forceCentered);
        }

        public static void DrawTextBox(Point point, Size2 size, Color bgcolor, Color txtcolor, string text, bool forceCentered = false)
        {
            theme.DrawTextBox(point, size, bgcolor, txtcolor, text, forceCentered);
        }

        public static void DrawBox(Point point, Size2 size)
        {
            DrawBox(point, size, BackgroundColor);
        }

        public static void DrawBox(Point point, Size2 size, Color color)
        {
            theme.DrawBox(point, size, color);
        }

        public static void DrawBorders(Point point, params Size2[] sizes)
        {
            DrawBorders(point, BorderColor, sizes);
        }

        public static void DrawBorders(Point point, Color bcolor, params Size2[] sizes)
        {
            theme.DrawBorders(point, bcolor, sizes);
        }

        #endregion

        private class NullTheme : ITheme
        {
            void IResumable.Start() => throw new InvalidOperationException();

            void IResumable.Pause() => throw new InvalidOperationException();

            Color ITheme.BackgroundColor { get; }

            Color ITheme.TextColor { get; }

            Color ITheme.BorderColor { get; }

            int ITheme.MinItemHeight { get; }

            Size2 ITheme.MeasureText(string text) => default(Size2);

            void ITheme.DrawBox(Point point, Size2 size, Color bgcolor)
            { }

            void ITheme.DrawTextBox(Point point, Size2 size, Color bgcolor, Color txtcolor, string text, bool forceCentered)
            { }

            void ITheme.DrawBorders(Point point, Color bcolor, params Size2[] sizes)
            { }
        }
    }
}