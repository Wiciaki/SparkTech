namespace SparkTech.SDK.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Color = SharpDX.Color;

    public interface ITheme : IDisposable
    {
        Color BackgroundColor { get; }

        int ItemGroupDistance { get; }

        Size MeasureText(string text);

        void DrawBox(Color color, Point point, Size size);

        void DrawTextBox(string text, Point point, Size size);

        void DrawBorders(IEnumerable<Size> sizes, Point point);
    }
}