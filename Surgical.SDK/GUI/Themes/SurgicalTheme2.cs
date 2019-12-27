﻿namespace Surgical.SDK.GUI.Themes
{
    using SharpDX;

    using Surgical.SDK.Rendering;

    public class SurgicalTheme2 : SurgicalTheme
    {
        public override void DrawBorders(Point point, Color color, params Size2[] sizes)
        {
            var p = new Vector2[3];

            var size = sizes[0];

            p[0] = new Vector2(point.X, point.Y + size.Height);
            p[1] = new Vector2(point.X, point.Y);
            p[2] = new Vector2(point.X + size.Width, point.Y);

            Vector.Draw(color, 1f, p);

            for (var i = 0; i < sizes.Length - 1; ++i)
            {
                point.Y += size.Height;

                size = sizes[i + 1];
            }

            p[0] = new Vector2(point.X, point.Y + size.Height);
            p[1] = new Vector2(point.X + size.Width, point.Y + size.Height);
            p[2] = new Vector2(point.X + size.Width, point.Y);

            Vector.Draw(color, 1f, p);
        }
    }
}