namespace SparkTech.UI.Default
{
    using System.Collections.Generic;
    using System.Drawing;

    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Mathematics.Interop;

    using Color = System.Drawing.Color;
    using Point = System.Drawing.Point;

    public class GrayTheme : EntropyTheme
    {
        public override Color BackgroundColor { get; } = Color.FromArgb(85, Color.Gray);

        public override Size ItemDistance { get; } = default;

        protected override FontDescription Description
        {
            get
            {
                var d = base.Description;
                d.FaceName = "Calibri";
                d.Height = 22;
                d.Weight = FontWeight.Light;
                return d;
            }
        }

        protected override void DrawText(string text, RawRectangle rect, FontDrawFlags flags, Color color, bool centered, bool bold)
        {
            if (!centered)
            {
                rect.Left += 8;
                rect.Right += 8;
            }
            else
            {
                flags &= ~FontDrawFlags.Left;
                flags |= FontDrawFlags.Center;
            }

            base.DrawText(text, rect, flags, color, centered, bold);
        }

        protected override FontDrawFlags Flags { get; } = FontDrawFlags.VerticalCenter | FontDrawFlags.Left;

        protected override IEnumerable<Vector2[]> GetBorderLines(Point point, Size size)
        {
            var p = new Vector2[5];

            p[0] = new Vector2(point.X, point.Y);
            p[1] = new Vector2(point.X, point.Y + size.Height);
            p[2] = new Vector2(point.X + size.Width, point.Y + size.Height);
            p[3] = new Vector2(point.X + size.Width, point.Y);
            p[4] = new Vector2(point.X, point.Y);

            yield return p;
        }

        protected override Color BorderColor { get; } = Color.WhiteSmoke;
    }
}