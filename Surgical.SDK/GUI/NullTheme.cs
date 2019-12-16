namespace Surgical.SDK.GUI
{
    using SharpDX;

    internal class NullTheme : ITheme
    {
        public void Dispose()
        { }

        public void Initialize()
        { }

        public Color BackgroundColor { get; }

        public int MinItemHeight { get; }

        public Size2 MeasureText(string text) => default;

        public void DrawBox(Point point, Size2 size, Color color)
        { }

        public void DrawTextBox(Point point, Size2 size, string text, bool forceCentered, Color? color)
        { }

        public void DrawBorders(Point point, params Size2[] sizes)
        { }
    }
}