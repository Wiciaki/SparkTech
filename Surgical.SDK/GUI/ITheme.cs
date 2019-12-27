namespace Surgical.SDK.GUI
{
    using SharpDX;

    public interface ITheme : IResumable
    {
        Color BackgroundColor { get; }

        Color BorderColor { get; }

        int MinItemHeight { get; }

        Size2 MeasureText(string text);

        void DrawBox(Point point, Color color, Size2 size);

        void DrawTextBox(Point point, Color color, Size2 size, string text, bool forceCentered, byte textAlpha);

        void DrawBorders(Point point, Color color, params Size2[] sizes);
    }
}