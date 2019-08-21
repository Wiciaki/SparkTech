namespace SparkTech.SDK.GUI
{
    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.Rendering;

    public class SharkTheme : ClassicTheme
    {
        protected override FontDrawFlags DrawFlags { get; } = FontDrawFlags.Left | FontDrawFlags.VerticalCenter;

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

        public override void DrawBorders(Point point, params Size2[] sizes)
        {
            foreach (var size in sizes)
            {
                var p = new Vector2[3];

                p[0] = new Vector2(point.X + size.Width, point.Y);
                p[1] = new Vector2(point.X, point.Y);
                p[2] = new Vector2(point.X, point.Y + size.Height);

                Vector.Draw(Color.WhiteSmoke, 2f, p);

                point.Y += size.Height;
            }
        }
    }
}