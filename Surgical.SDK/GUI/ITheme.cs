namespace Surgical.SDK.GUI
{
    using System;

    using SharpDX;

    public interface ITheme : IDisposable
    {
        void Initialize();

        Color BackgroundColor { get; }

        int MinItemHeight { get; }

        Size2 MeasureText(string text);

        void DrawBox(Point point, Size2 size, Color color);

        void DrawTextBox(Point point, Size2 size, string text, bool forceCentered, Color? color);

        void DrawBorders(Point point, params Size2[] sizes);
    }
}