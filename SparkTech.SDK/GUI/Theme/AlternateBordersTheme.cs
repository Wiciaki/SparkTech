namespace SparkTech.SDK.GUI.Themes
{
    using SharpDX;

    using SparkTech.SDK.Rendering;

    public class AlternateBordersTheme : DefaultTheme
    {
        protected AlternateBordersTheme()
        {
        }

        public override void DrawBorders(Point point, Color color, params Size2[] sizes)
        {
            const float Thickness = 1f;

            var p = new Vector2[3];

            var size = sizes[0];

            p[0] = new Vector2(point.X, point.Y + size.Height);
            p[1] = new Vector2(point.X, point.Y);
            p[2] = new Vector2(point.X + size.Width, point.Y);

            Vector.Draw(color, Thickness, p);

            for (var i = 0; i < sizes.Length - 1; ++i)
            {
                point.Y += size.Height;

                size = sizes[i + 1];
            }

            p[0] = new Vector2(point.X, point.Y + size.Height);
            p[1] = new Vector2(point.X + size.Width, point.Y + size.Height);
            p[2] = new Vector2(point.X + size.Width, point.Y);

            Vector.Draw(color, Thickness, p);
        }

        public new static ITheme Create()
        {
            return new AlternateBordersTheme();
        }
    }
}