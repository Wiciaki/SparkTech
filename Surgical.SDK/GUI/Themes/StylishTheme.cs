namespace Surgical.SDK.GUI.Themes
{
    using SharpDX;
    using SharpDX.Direct3D9;

    using Surgical.SDK.Rendering;

    public class StylishTheme : DefaultTheme
    {
        public StylishTheme(Color color)
        {
            color.A -= 100;

            this.BackgroundColor = color;
        }

        public override Color BackgroundColor { get; }

        public override FontDescription GetFontDescription()
        {
            return new FontDescription { FaceName = "Cambria", Height = 18 };
        }

        public override void DrawBorders(Point point, Color color, params Size2[] sizes)
        {
            foreach (var size in sizes)
            {
                var p = new[] { new Vector2(point.X + size.Width, point.Y), new Vector2(point.X, point.Y), new Vector2(point.X, point.Y + size.Height) };

                Vector.Draw(color, 2f, p);

                point.Y += size.Height;
            }
        }
    }
}