namespace SparkTech.SDK.GUI
{
    using System.Collections.Generic;
    using System.Drawing;

    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Mathematics.Interop;

    using SparkTech.SDK.Misc;
    using SparkTech.SDK.Rendering;

    using Color = SharpDX.Color;
    using Font = SharpDX.Direct3D9.Font;
    using Point = System.Drawing.Point;

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

        private readonly Size extraTextSize = new Size(10,16);

        public Size MeasureText(string text)
        {
            var r = this.font.MeasureText(null, text, DrawFlags);

            var height = MultipleOf(r.Bottom - r.Top + this.extraTextSize.Height, 28);
            var width = MultipleOf(r.Right - r.Left + this.extraTextSize.Width, 2);

            return new Size(width, height);

            static int MultipleOf(int num, int multipleOf)
            {
                var mod = num % multipleOf;

                return mod == 0 ? num : num + multipleOf - mod;
            }
        }

        public void DrawTextBox(Point point, Size size, string text, Color? color)
        {
            this.DrawBox(point, size, color ?? this.BackgroundColor);

            this.font.DrawText(null, text, this.GetTextRectangle(point, size), DrawFlags, Color.White);
        }

        public void DrawBox(Point point, Size size, Color color)
        {
            Vector.Draw(color, size.Height, point.ToVector2(), new Vector2(point.X + size.Width, point.Y));
        }

        public void DrawBorders(IEnumerable<Size> sizes, Point point)
        {

        }

        private RawRectangle GetTextRectangle(Point point, Size size)
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