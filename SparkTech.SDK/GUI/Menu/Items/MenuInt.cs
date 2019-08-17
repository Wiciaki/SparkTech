namespace SparkTech.SDK.GUI.Menu.Items
{
    using System;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Rendering;

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

        private Size2 size;

        protected override Size2 GetSize()
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
                var diff = GameInterface.CursorPosition().X - point.X;

                if (diff <= 0)
                {
                    displayNum = this.Min;
                }
                else if (diff >= barWidth)
                {
                    displayNum = this.Max;
                }
                else
                {
                    displayNum = this.Min + (int)((float)diff * range / barWidth);
                }

                this.draggingVal = displayNum;
            }

            point.X += width;
            Theme.DrawTextBox(point, this.size, $"[{displayNum}]");
            point.X -= width;

            point.Y += this.size.Height;
            Theme.DrawBox(point, new Size2(barWidth, SliderHeight), Theme.BackgroundColor);

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

            if (!this.dragging && !Menu.IsCursorInside(point, new Size2(width, SliderHeight)))
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