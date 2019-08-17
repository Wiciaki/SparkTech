namespace SparkTech.SDK.GUI
{
    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Mathematics.Interop;

    using SparkTech.SDK.Rendering;

    public sealed class ClassicTheme : ITheme
    {
        private readonly Font font;

        public ClassicTheme()
        {
            var desc = new FontDescription { FaceName = "Calibri", Height = 12 };

            this.font = new Font(Render.Direct3DDevice, desc);

            Render.OnLostDevice += this.font.OnLostDevice;
            Render.OnResetDevice += this.font.OnResetDevice;
            Render.OnDispose += this.Dispose;
        }

        public Color BackgroundColor { get; } = Color.Gray;

        public int ItemGroupDistance { get; } = 10;

        private const FontDrawFlags DrawFlags = FontDrawFlags.VerticalCenter | FontDrawFlags.Left;

        private readonly Size2 extraTextSize = new Size2(10,16);

        public Size2 MeasureText(string text)
        {
            var r = this.font.MeasureText(null, text, DrawFlags);

            var height = MultipleOf(r.Bottom - r.Top + this.extraTextSize.Height, 28);
            var width = MultipleOf(r.Right - r.Left + this.extraTextSize.Width, 2);

            return new Size2(width, height);

            static int MultipleOf(int num, int multipleOf)
            {
                var mod = num % multipleOf;

                return mod == 0 ? num : num + multipleOf - mod;
            }
        }

        public void DrawTextBox(Point point, Size2 size, string text, Color? color)
        {
            this.DrawBox(point, size, color ?? this.BackgroundColor);

            this.font.DrawText(null, text, this.GetTextRectangle(point, size), DrawFlags, Color.White);
        }

        public void DrawBox(Point point, Size2 size, Color color)
        {
            Vector.Draw(color, size.Height, point, new Point(point.X + size.Width, point.Y));
        }

        public void DrawBorders(Point point, params Size2[] sizes)
        {

        }

        private RawRectangle GetTextRectangle(Point point, Size2 size)
        {
            var ew = this.extraTextSize.Width / 2;
            var eh = this.extraTextSize.Height / 2;

            return new RawRectangle(point.X + ew, point.Y + eh, point.X + size.Width - ew, point.Y + size.Height - eh);
        }

        public void Dispose()
        {
            this.font.Dispose();
        }

        /*
        FontDescription ITheme.GetFontDescription()
        {
            var desc = new FontDescription();

            desc.FaceName = "Calibri";
            desc.Height = 12;

            return desc;
        }

        Size ITheme.ExtraTextBoxSize()
        {
            return new Size(20, 20);
        }

        public Color GetFontColor()
        {
            throw new System.NotImplementedException();
        }

        Color ITheme.GetBackgroundColor()
        {
            return Color.Gray;
        }

        Color? ITheme.GetBorderColor()
        {
            return Color.Black;
        }

        int ITheme.GetItemGroupDistance()
        {
            return 15;
        }

        int ITheme.GetMinItemHeight()
        {
            return 25;
        }*/
    }
}