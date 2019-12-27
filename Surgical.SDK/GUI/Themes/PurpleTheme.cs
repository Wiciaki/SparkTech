namespace Surgical.SDK.GUI.Themes
{
    using SharpDX;
    using SharpDX.Direct3D9;

    using Surgical.SDK.Rendering;

    public class PurpleTheme : SurgicalTheme
    {
        public override Color BorderColor { get; } = Color.DarkGray;

        public override Color BackgroundColor
        {
            get
            {
                var c = Color.Purple;
                c.A = 120;
                return c;
            }
        }

        public override FontDescription GetFontDescription()
        {
            return new FontDescription { FaceName = "Arial", Height = 16 };
        }

        public override void DrawBorders(Point point, Color color, params Size2[] sizes)
        {
            foreach (var size in sizes)
            {
                var p = new[] { new Vector2(point.X + size.Width, point.Y), new Vector2(point.X, point.Y), new Vector2(point.X, point.Y + size.Height) };

                Vector.Draw(color, 1.5f, p);

                point.Y += size.Height;
            }
        }
    }
}