namespace Surgical.SDK.GUI.Themes
{
    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Mathematics.Interop;

    using Surgical.SDK.Rendering;

    public class SurgicalTheme : ITheme
    {
        public virtual Color BackgroundColor { get; }

        public virtual Color BorderColor { get; }

        public int MinItemHeight { get; } = 26;

        public SurgicalTheme()
        {
            var color = Color.Black;
            color.A = 130;

            this.BackgroundColor = color;
            this.BorderColor = Color.White;
        }

        protected Font Font { get; private set; }

        public virtual FontDescription GetFontDescription()
        {
            return new FontDescription { FaceName = "DengXian", Height = 18 };
        }

        private const FontDrawFlags DrawFlags = FontDrawFlags.VerticalCenter | FontDrawFlags.Left;

        private const FontDrawFlags CenteredFlags = FontDrawFlags.VerticalCenter | FontDrawFlags.Center;

        private readonly Size2 extraTextSize = new Size2(12, 4);

        public Size2 MeasureText(string text)
        {
            var r = this.Font.MeasureText(null, text, DrawFlags);

            var size = this.extraTextSize;

            size.Height += MultipleOf(r.Bottom - r.Top, this.MinItemHeight);
            size.Width += MultipleOf(r.Right - r.Left, 2);

            return size;
        }

        private static int MultipleOf(int num, int multipleOf)
        {
            var mod = num % multipleOf;

            return mod == 0 ? num : num + multipleOf - mod;
        }

        public void DrawBox(Point point, Color color, Size2 size)
        {
            point.Y += size.Height / 2;

            Vector.Draw(color, size.Height, point, new Point(point.X + size.Width, point.Y));
        }

        public void DrawTextBox(Point point, Color color, Size2 size, string text, bool forceCentered, byte textAlpha)
        {
            this.DrawBox(point, color, size);

            var textColor = Color.White;
            textColor.A = textAlpha;

            this.Font.DrawText(null, text, this.GetTextRectangle(point, size), forceCentered ? CenteredFlags : DrawFlags, textColor);
        }

        public virtual void DrawBorders(Point point, Color color, params Size2[] sizes)
        {
            foreach (var size in sizes)
            {
                var p = new Vector2[5];

                p[0] = new Vector2(point.X, point.Y);
                p[1] = new Vector2(point.X, point.Y + size.Height);
                p[2] = new Vector2(point.X + size.Width, point.Y + size.Height);
                p[3] = new Vector2(point.X + size.Width, point.Y);
                p[4] = new Vector2(point.X, point.Y);

                Vector.Draw(color, 1f, p);

                point.Y += size.Height;
            }
        }

        //public override void DrawBorders(Point point, params Size2[] sizes)
        //{
        //    var p = new Vector2[3];

        //    var size = sizes[0];
                
        //    p[0] = new Vector2(point.X, point.Y + size.Height);
        //    p[1] = new Vector2(point.X, point.Y);
        //    p[2] = new Vector2(point.X + size.Width, point.Y);

        //    Vector.Draw(Color.White, 1f, p);

        //    for (var i = 0; i < sizes.Length - 1; ++i)
        //    {
        //        point.Y += size.Height;

        //        size = sizes[i + 1];
        //    }

        //    p[0] = new Vector2(point.X, point.Y + size.Height);
        //    p[1] = new Vector2(point.X + size.Width, point.Y + size.Height);
        //    p[2] = new Vector2(point.X + size.Width, point.Y);

        //    Vector.Draw(Color.White, 1f, p);
        //}

        private RawRectangle GetTextRectangle(Point point, Size2 size)
        {
            var w = this.extraTextSize.Width / 2;
            var h = this.extraTextSize.Height / 2;

            return new RawRectangle(point.X + w, point.Y + h, point.X + size.Width - w, point.Y + size.Height - h);
        }

        public void Start()
        {
            this.Font = new Font(Render.Device, this.GetFontDescription());

            Render.OnLostDevice += this.Font.OnLostDevice;
            Render.OnResetDevice += this.Font.OnResetDevice;
            Render.OnDispose += this.Font.Dispose;
        }

        public void Pause()
        {
            Render.OnLostDevice -= this.Font.OnLostDevice;
            Render.OnResetDevice -= this.Font.OnResetDevice;
            Render.OnDispose -= this.Font.Dispose;

            this.Font.Dispose();
            this.Font = null;
        }
    }
}