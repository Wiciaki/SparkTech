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
            void IResumable.Start() => throw new InvalidOperationException();

            void IResumable.Pause() => throw new InvalidOperationException();

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
            theme = PlatformTheme ?? DefaultTheme.Create();
            theme.Start();

            Menu.Menu.UpdateDecalSizes();
            Clock.UpdateSize();

        }

        internal static void SetTheme(int i)
        {
            theme.Pause();

            theme = i switch
            {
                0 => PlatformTheme ?? throw new InvalidOperationException(),
                1 => DefaultTheme.Create(),
                2 => AlternateBordersTheme.Create(),
                3 => SimpleTheme.CreateBlack(),
                4 => SimpleTheme.CreateWhite(),
                5 => SimpleTheme.CreateBlue(),
                6 => StylishTheme.CreateRed(),
                7 => StylishTheme.CreateBlue(),
                _ => throw new IndexOutOfRangeException()
            };

            theme.Start();

            Menu.Menu.UpdateDecalSizes();
            Clock.UpdateSize();

            Menu.Menu.UpdateAllSizes();
            Notifications.Notification.UpdateAllSizes();
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
    }
}