namespace SparkTech.SDK.GUI.Default
{
    using SharpDX;

    using SparkTech.SDK.Rendering;

    public class Theme2 : Theme
    {
        public override void DrawBorders(Point point, params Size2[] sizes)
        {
            var p = new Vector2[3];

            var size = sizes[0];
                
            p[0] = new Vector2(point.X, point.Y + size.Height);
            p[1] = new Vector2(point.X, point.Y);
            p[2] = new Vector2(point.X + size.Width, point.Y);

            Vector.Draw(Color.White, 1f, p);

            for (var i = 0; i < sizes.Length - 1; ++i)
            {
                point.Y += size.Height;

                size = sizes[i + 1];
            }

            p[0] = new Vector2(point.X, point.Y + size.Height);
            p[1] = new Vector2(point.X + size.Width, point.Y + size.Height);
            p[2] = new Vector2(point.X + size.Width, point.Y);

            Vector.Draw(Color.White, 1f, p);
        }
    }
}