namespace SparkTech.SDK.GUI
{
    using System;

    using SharpDX;

    public interface ITheme : IDisposable
    {
        Color BackgroundColor { get; }

        int ItemGroupDistance { get; }

        Size2 MeasureText(string text);

        void DrawBox(Point point, Size2 size, Color color);

        void DrawTextBox(Point point, Size2 size, string text, Color? color);

        void DrawBorders(Point point, params Size2[] sizes);
    }
}