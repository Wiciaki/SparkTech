namespace SparkTech.SDK.GUI.Menu
{
    using System;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.Rendering;

    public class MenuFloat : MenuValue, IMenuValue<float>
    {
        #region Fields

        private float value, draggingVal;

        private bool dragging;

        #endregion

        #region Constructors and Destructors

        public MenuFloat(string id, float from, float to, float defaultValue) : this(id, from, to, (JToken)defaultValue)
        {
            this.CheckBounds(defaultValue);
        }

        protected MenuFloat(string id, float from, float to, JToken defaultValue) : base(id, defaultValue)
        {
            this.From = from;

            this.To = to;
        }

        public float From { get; }

        public float To { get; }

        public float Value
        {
            get => this.value;
            set
            {
                this.CheckBounds(value);

                if (Math.Abs(this.value - value) >= 0.01f && this.InvokeNotifier(value))
                {
                    this.value = value;
                }
            }
        }

        public float Min => Math.Min(this.From, this.To);

        public float Max => Math.Max(this.From, this.To);

        protected virtual bool InvokeNotifier(float num)
        {
            return this.UpdateValue(num);
        }

        protected void CheckBounds(float num)
        {
            if (num > this.Max)
            {
                throw new ArgumentException("num > max", nameof(num));
            }

            if (num < this.Min)
            {
                throw new ArgumentException("num < min", nameof(num));
            }
        }

        #endregion

        #region Properties

        protected virtual string GetMaxNumStr()
        {
            return $"[{(int)this.Max}.00]";
        }

        protected virtual string GetPrintableStr(float num)
        {
            return $"[{num:F}]";
        }

        private Size2 size;

        protected override Size2 GetSize()
        {
            this.size = Theme.MeasureText(this.GetMaxNumStr());

            var s = base.GetSize();

            s.Height += Theme.MinItemHeight;
            s.Width += this.size.Width;

            return s;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            var barWidth = width;
            var barHeight = Theme.MinItemHeight;
            var displayNum = this.Value;
            var range = this.Max - this.Min;

            width -= this.size.Width;
            base.OnEndScene(point, width);

            if (this.dragging)
            {
                var diff = 0;//GameInterface.CursorPosition().X - point.X;

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
                    displayNum = this.Min + diff * range / barWidth;
                }

                this.draggingVal = displayNum;
            }

            point.X += width;
            Theme.DrawTextBox(point, this.size, this.GetPrintableStr(displayNum), true);
            point.X -= width;

            point.Y += this.size.Height;

            var s = new Size2(this.size.Width, barHeight);
            Theme.DrawTextBox(point, s, $"{this.From:0.##}", true);
            point.X += s.Width;

            var drawSize = new Size2(barWidth - 2 * s.Width, barHeight);
            Theme.DrawBox(point, drawSize, Theme.BackgroundColor);
            
            point.X += drawSize.Width;
            Theme.DrawTextBox(point, s, $"{this.To:0.##}", true);
            point.X -= drawSize.Width + s.Width;

            var offset = (int)(barWidth / (range / (this.Value - this.Min)));

            if (this.From > this.To)
            {
                offset = barWidth - offset;
            }

            var color = Color.White;

            if (!this.dragging)
            {
                color.A = 150;
            }

            point.Y += barHeight / 2;
            Vector.Draw(color, barHeight, point, new Point(point.X + offset, point.Y));
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            if (!this.dragging)
            {
                point.Y += this.size.Height;

                if (!Menu.IsCursorInside(point, new Size2(width, Theme.MinItemHeight)))
                {
                    return;
                }
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
            get => this.Value;
            set => this.Value = value.Value<float>();
        }

        #endregion
    }
}