namespace SparkTech.SDK.GUI
{
    using System;
    using System.Drawing;

    using Color = SharpDX.Color;

    public interface ITheme : IDisposable
    {
        Color BackgroundColor { get; }

        int ItemGroupDistance { get; }

        Size MeasureText(string text);

        void DrawBox(Point point, Size size, Color color);

        void DrawTextBox(Point point, Size size, string text, Color? color);

        void DrawBorders(Point point, params Size[] sizes);
    }
}