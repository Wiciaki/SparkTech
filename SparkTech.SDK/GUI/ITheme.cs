namespace SparkTech.SDK.GUI
{
    using Modules;

    using SharpDX;

    public interface ITheme : IResumable
    {
        Color BackgroundColor { get; }

        Color TextColor { get; }

        Color BorderColor { get; }

        int MinItemHeight { get; }

        Size2 MeasureText(string text);

        void DrawBox(Point point, Size2 size, Color bgcolor);

        void DrawTextBox(Point point, Size2 size, Color bgcolor, Color txtcolor, string text, bool forceCentered);

        void DrawBorders(Point point, Color bcolor, params Size2[] sizes);
    }
}