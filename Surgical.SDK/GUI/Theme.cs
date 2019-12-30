namespace Surgical.SDK.GUI
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using SharpDX;

    using Surgical.SDK.GUI.Themes;

    public static class Theme
    {
        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty", Justification = "This class provides an empty implementation")]
        private class NullTheme : ITheme
        {
            void IResumable.Pause()
            { }

            void IResumable.Start()
            { }

            Color ITheme.BackgroundColor { get; }

            Color ITheme.TextColor { get; }

            Color ITheme.BorderColor { get; }

            int ITheme.MinItemHeight { get; }

            Size2 ITheme.MeasureText(string text) => default;

            void ITheme.DrawBox(Point point, Size2 size, Color bgcolor)
            { }

            void ITheme.DrawTextBox(Point point, Size2 size, Color bgcolor, Color txtcolor, string text, bool forceCentered)
            { }

            void ITheme.DrawBorders(Point point, Color bcolor, params Size2[] sizes)
            { }
        }

        private static readonly ITheme? PlatformTheme;

        private static ITheme theme;

        static Theme()
        {
            if (!Platform.HasRenderAPI)
            {
                theme = new NullTheme();
                return;
            }

            PlatformTheme = Platform.PlatformTheme;
            theme = PlatformTheme ?? new DefaultTheme();

            theme.Start();

            Menu.Menu.UpdateArrowSize();
            Clock.UpdateSize();
        }

        internal static void SetTheme(int i)
        {
            theme.Pause();

            theme = i switch
            {
                0 => PlatformTheme ?? throw new InvalidOperationException(),
                1 => new DefaultTheme(),
                2 => new AlternateBordersTheme(),
                3 => new SimpleTheme(Color.White, Color.Black),
                4 => new SimpleTheme(Color.Black, Color.White),
                5 => new StylishTheme(Color.DarkRed),
                6 => new StylishTheme(Color.DarkBlue),
                _ => throw new IndexOutOfRangeException()
            };

            theme.Start();

            Menu.Menu.UpdateArrowSize();
            Clock.UpdateSize();

            Menu.Menu.UpdateAllSizes();
            Notifications.Notification.UpdateAllSizes();
        }

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
    }
}