﻿namespace SparkTech.SDK.GUI
{
    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Mathematics.Interop;

    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Rendering;

    public class ClassicTheme : ITheme
    {
        private Font font;

        public ClassicTheme()
        {
            //Render.OnLostDevice += this.font.OnLostDevice;
            //Render.OnResetDevice += this.font.OnResetDevice;
            //Render.OnDispose += this.Dispose;
        }

        protected Font Font
        {
            get
            {
                return this.font ??= new Font(Render.Direct3DDevice, this.GetFontDescription());
            }
        }

        public virtual FontDescription GetFontDescription() => new FontDescription { FaceName = "Calibri", Height = 18 };

        public virtual Color BackgroundColor
        {
            get
            {
                var c = Color.Black;
                c.A = 120;
                return c;
            }
        }

        public int MinItemHeight { get; } = 28;

        public int ItemGroupDistance { get; } = 0;

        protected virtual FontDrawFlags DrawFlags { get; } = FontDrawFlags.VerticalCenter | FontDrawFlags.Center;

        private const FontDrawFlags CenteredFlags = FontDrawFlags.VerticalCenter | FontDrawFlags.Center;

        private readonly Size2 extraTextSize = new Size2(10,0);

        public Size2 MeasureText(string text)
        {
            var r = this.Font.MeasureText(null, text, DrawFlags);

            var height = MultipleOf(r.Bottom - r.Top + this.extraTextSize.Height, this.MinItemHeight);
            var width = MultipleOf(r.Right - r.Left + this.extraTextSize.Width, 2);

            return new Size2(width + this.extraTextSize.Width, height + this.extraTextSize.Height);

            static int MultipleOf(int num, int multipleOf)
            {
                var mod = num % multipleOf;

                return mod == 0 ? num : num + multipleOf - mod;
            }
        }

        public void DrawTextBox(Point point, Size2 size, string text, bool centered, Color? color)
        {
            this.DrawBox(point, size, color ?? this.BackgroundColor);

            this.Font.DrawText(null, text, this.GetTextRectangle(point, size), centered ? CenteredFlags : this.DrawFlags, Color.White);
        }

        public void DrawBox(Point point, Size2 size, Color color)
        {
            point.Y += size.Height / 2;
            Vector.Draw(color, size.Height, point, new Point(point.X + size.Width, point.Y));
        }

        public virtual void DrawBorders(Point point, params Size2[] sizes)
        {
            foreach (var size in sizes)
            {
                var p = new Vector2[5];

                p[0] = new Vector2(point.X, point.Y);
                p[1] = new Vector2(point.X, point.Y + size.Height);
                p[2] = new Vector2(point.X + size.Width, point.Y + size.Height);
                p[3] = new Vector2(point.X + size.Width, point.Y);
                p[4] = new Vector2(point.X, point.Y);

                Vector.Draw(Color.White, 1f, p);

                point.Y += size.Height;
            }
        }

        private RawRectangle GetTextRectangle(Point point, Size2 size)
        {
            var w = this.extraTextSize.Width / 2;
            var h = this.extraTextSize.Height / 2;

            return new RawRectangle(point.X + w, point.Y + h, point.X + size.Width - w, point.Y + size.Height - h);
        }

        public void Dispose()
        {
            this.Font.Dispose();
        }
    }
}