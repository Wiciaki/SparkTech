namespace SparkTech.UI.Default
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using SparkTech.UI.Menu;
    using SparkTech.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Mathematics.Interop;

    using Color = System.Drawing.Color;
    using Point = System.Drawing.Point;

    public class EntropyTheme : ITheme
    {
        ModuleMenu IEntropyModule.Menu => null;

        public virtual void Release()
        {
            this.Font.Dispose();
        }

        private SharpDX.Direct3D9.Font font, boldFont;

        protected virtual SharpDX.Direct3D9.Font Font =>
            this.font ?? (this.font = new SharpDX.Direct3D9.Font(Renderer.Direct3DDevice, this.Description));

        private SharpDX.Direct3D9.Font BoldFont
        {
            get
            {
                if (this.boldFont != null)
                {
                    return this.boldFont;
                }

                var d = this.Description;
                d.Weight = FontWeight.ExtraBold;

                return this.boldFont = new SharpDX.Direct3D9.Font(Renderer.Direct3DDevice, d);
            }
        }

        protected virtual FontDescription Description => new FontDescription { FaceName = "Tahoma", Quality = FontQuality.Antialiased, Weight = FontWeight.Bold, Height = 20, PitchAndFamily = FontPitchAndFamily.DontCare, CharacterSet = FontCharacterSet.Ansi };

        protected virtual FontDrawFlags Flags { get; } = FontDrawFlags.Center | FontDrawFlags.VerticalCenter;

        public virtual Color BackgroundColor { get; } = Color.FromArgb(85, Color.Purple);

        public virtual Color FontColor => Color.WhiteSmoke;

        protected virtual Color BorderColor => Color.Black;

        private readonly float scale = Renderer.ScreenResolutionX / 1920f;

        public virtual Size ItemDistance { get; } = new Size(12, 0);

        public virtual Size MeasureSize(string text)
        {
            var r = this.Font.MeasureText(null, text, this.Flags);

            return new Size(r.Right - r.Left + 16, Math.Max(r.Bottom - r.Top + 10, 28));
        }

        public void Draw(DrawData data)
        {
            var leftB = data.Point;
            leftB.Y += data.Size.Height / 2;

            var rightB = leftB;
            rightB.X += data.Size.Width;

            Rendering.Line.Render(
                data.BackgroundColor.ToSharpDXColor(),
                data.Size.Height,
                true,
                new RawVector2(leftB.X, leftB.Y),
                new RawVector2(rightB.X, rightB.Y));

            foreach (var pos in this.GetBorderLines(data.Point, data.Size))
            {
                Rendering.Line.Render(Color.FromArgb(data.FontColor.A, this.BorderColor).ToSharpDXColor(), 1, true, pos);
            }

            if (data.Text != null)
            {
                var rect = new RawRectangle(
                    data.Point.X,
                    data.Point.Y,
                    data.Point.X + data.Size.Width,
                    data.Point.Y + data.Size.Height);

                this.DrawText(data.Text, rect, this.Flags, data.FontColor, data.ForceTextCentered, data.Bold);
            }
        }

        protected virtual IEnumerable<Vector2[]> GetBorderLines(Point point, Size size)
        {
            var pos = new Vector2[2];

            var rightBound = point.X + size.Width;
            var topBound = point.Y + size.Height;

            pos[0] = new Vector2(point.X, point.Y);
            pos[1] = new Vector2(rightBound, point.Y);

            yield return pos;

            pos[0] = new Vector2(point.X, topBound);
            pos[1] = new Vector2(rightBound, topBound);

            yield return pos;
        }

        protected virtual void DrawText(string text, RawRectangle rect, FontDrawFlags flags, Color color, bool centered, bool bold)
        {
            (bold ? this.BoldFont : this.Font).DrawText(null, text, rect, flags, color.ToSharpDXColor());
        }
    }
}