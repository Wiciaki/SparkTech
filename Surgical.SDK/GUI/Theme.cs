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

            Color ITheme.BorderColor { get; }

            int ITheme.MinItemHeight { get; }

            Size2 ITheme.MeasureText(string text) => default;

            void ITheme.DrawBox(Point point, Color color, Size2 size)
            { }

            void ITheme.DrawTextBox(Point point, Color color, Size2 size, string text, bool forceCentered, byte textAlpha)
            { }

            void ITheme.DrawBorders(Point point, Color color, params Size2[] sizes)
            { }
        }

        private static ITheme theme, platformTheme;

        internal static void Initialize(ITheme t)
        {
            platformTheme = t;

            if (!Platform.HasRenderAPI)
            {
                theme = new NullTheme();
                return;
            }

            theme = t ?? new SurgicalTheme();
            theme.Start();

            Menu.Menu.UpdateArrowSize();
            Clock.UpdateSize();
        }

        internal static void SetTheme(int i)
        {
            theme.Pause();

            theme = i switch
            {
                0 => platformTheme,
                1 => new SurgicalTheme(),
                2 => new SurgicalTheme2(),
                3 => new PurpleTheme(),
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

        public static Size2 MeasureText(string text)
        {
            return theme.MeasureText(text);
        }

        public static void DrawTextBox(Point point, Size2 size, string text, bool forceCentered = false)
        {
            DrawTextBox(point, size, BackgroundColor, text, forceCentered);
        }

        public static void DrawTextBox(Point point, Size2 size, Color color, string text, bool forceCentered = false, byte textAlpha = byte.MaxValue)
        {
            theme.DrawTextBox(point, color, size, text, forceCentered, textAlpha);
        }

        public static void DrawBox(Point point, Size2 size, Color color)
        {
            theme.DrawBox(point, color, size);
        }

        public static void DrawBorders(Point point, params Size2[] sizes)
        {
            theme.DrawBorders(point, BorderColor, sizes);
        }

        public static void DrawBorders(Point point, Color color, params Size2[] sizes)
        {
            theme.DrawBorders(point, color, sizes);
        }
    }
}