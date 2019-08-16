namespace SparkTech.SDK.GUI.Menu.Items
{
    using System;
    using System.Drawing;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Rendering;

    using Color = SharpDX.Color;

    public class MenuSlider : MenuValue, IMenuValue<int>
    {
        #region Fields

        private int value, draggingVal;

        private bool dragging;

        #endregion

        #region Constructors and Destructors

        public MenuSlider(string id, int min, int max, int defaultValue) : base(id, defaultValue)
        {
            if (min > max)
            {
                throw new ArgumentException("The specified min was greater than max");
            }

            this.Min = min;

            this.Max = max;

            this.CheckBounds(defaultValue);
        }

        private void CheckBounds(int v)
        {
            if (v < this.Min || v > this.Max)
            {
                throw new ArgumentException("The specified value was out of range");
            }
        }

        #endregion

        #region Public Properties

        public readonly int Max;

        public readonly int Min;

        public int Value
        {
            get => this.value;
            set
            {
                if (value == this.value)
                {
                    return;
                }

                this.CheckBounds(value);

                if (this.UpdateValue(value))
                {
                    this.value = value;
                }
            }
        }

        #endregion

        #region Properties

        private const int SliderHeight = 20;

        private Size size;

        protected override Size GetSize()
        {
            var s = base.GetSize();
            s.Height += SliderHeight;

            this.size = Theme.MeasureText($"[{this.Max}]");

            s.Width += this.size.Width;

            return s;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            var barWidth = width;
            var displayNum = this.Value;
            var range = this.Max - this.Min;

            width -= this.size.Width;

            base.OnEndScene(point, width);

            if (this.dragging)
            {
                var x = (int)GameInterface.CursorPosition().X;

                var diff = x - point.X;

                displayNum = diff <= 0 ? this.Min : x >= point.X + barWidth ? this.Max : this.Min + (int)((float)diff * range / barWidth);

                this.draggingVal = displayNum;
            }

            point.X += width;
            Theme.DrawTextBox($"[{displayNum}]", point, this.size);
            point.X -= width;

            point.Y += this.size.Height;
            Theme.DrawBox(Theme.BackgroundColor, point, new Size(barWidth, SliderHeight));

            var offset = (int)(barWidth / ((float)range / (this.Value - this.Min)));

            var color = Color.White;

            if (!this.dragging)
            {
                color.A = 150;
            }

            Vector.Draw(color, SliderHeight, point, new Point(point.X + offset, point.Y));
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            point.Y += this.size.Height;

            if (!this.dragging && !Menu.IsCursorInside(point, new Size(width, SliderHeight)))
            {
                return;
            }

            if (Menu.IsLeftClick(args.Message))
            {
                this.dragging = true;
            }
            else if (args.Message == WindowsMessages.LBUTTONUP)
            {
                this.dragging = false;

                this.Value = this.draggingVal;
            }
        }

        protected override JToken Token
        {
            get => this.value;
            set => this.value = value.Value<int>();
        }

        #endregion
    }
}