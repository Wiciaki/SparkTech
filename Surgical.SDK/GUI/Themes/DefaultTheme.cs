namespace Surgical.SDK.GUI.Themes
{
    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Mathematics.Interop;

    using Surgical.SDK.Rendering;

    public class DefaultTheme : ITheme
    {
        public virtual Color BackgroundColor { get; }

        public virtual Color TextColor { get; }

        public virtual Color BorderColor { get; }

        public int MinItemHeight { get; } = 26;

        protected DefaultTheme()
        {
            var color = Color.Black;
            color.A = 130;

            this.BackgroundColor = color;
            this.BorderColor = Color.White;
            this.TextColor = Color.White;
        }

        public static ITheme Create() => new DefaultTheme();

        protected Font? Font { get; private set; }

        public virtual FontDescription GetFontDescription()
        {
            return new FontDescription { FaceName = string.Empty, Height = 18, OutputPrecision = FontPrecision.TrueType, Quality = FontQuality.ClearType };
        }

        private const FontDrawFlags DrawFlags = FontDrawFlags.VerticalCenter | FontDrawFlags.Left;

        private const FontDrawFlags CenteredFlags = FontDrawFlags.VerticalCenter | FontDrawFlags.Center;

        protected virtual Size2 ExtraTextSize { get; } = new Size2(12, 4);

        public Size2 MeasureText(string text)
        {
            static int MultipleOf(int num, int multipleOf)
            {
                var mod = num % multipleOf;

                return mod == 0 ? num : num + multipleOf - mod;
            }

            var r = this.Font!.MeasureText(null, text, DrawFlags);

            var size = this.ExtraTextSize;

            size.Height += MultipleOf(r.Bottom - r.Top, this.MinItemHeight);
            size.Width += MultipleOf(r.Right - r.Left, 2);

            return size;
        }

        public virtual void DrawBox(Point point, Size2 size, Color color)
        {
            if (color == default)
            {
                return;
            }

            point.Y += size.Height / 2;

            Vector.Draw(color, size.Height, point, new Point(point.X + size.Width, point.Y));
        }

        public virtual void DrawTextBox(Point point, Size2 size, Color bgcolor, Color txtcolor, string text, bool forceCentered)
        {
            this.DrawBox(point, size, bgcolor);

            var flags = forceCentered ? CenteredFlags : DrawFlags;
            var rect = this.GetTextRectangle(point, size);

            this.Font!.DrawText(null, text, rect, flags, txtcolor);
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
                p[4] = p[0];

                Vector.Draw(color, 1f, p);

                point.Y += size.Height;
            }
        }

        protected RawRectangle GetTextRectangle(Point point, Size2 size)
        {
            var w = this.ExtraTextSize.Width / 2;
            var h = this.ExtraTextSize.Height / 2;

            return new RawRectangle(point.X + w, point.Y + h, point.X + size.Width - w, point.Y + size.Height - h);
        }

        public virtual void Start()
        {
            this.Font = new Font(Render.Device, this.GetFontDescription());

            Render.OnLostDevice += this.Font.OnLostDevice;
            Render.OnResetDevice += this.Font.OnResetDevice;
            Render.OnDispose += this.Font.Dispose;
        }

        public virtual void Pause()
        {
            Render.OnLostDevice -= this.Font!.OnLostDevice;
            Render.OnResetDevice -= this.Font.OnResetDevice;
            Render.OnDispose -= this.Font.Dispose;

            this.Font.Dispose();
            this.Font = null;
        }
    }
}