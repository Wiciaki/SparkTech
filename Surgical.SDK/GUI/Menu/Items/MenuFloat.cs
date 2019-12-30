namespace Surgical.SDK.GUI.Menu
{
    using System;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    using Surgical.SDK.EventData;

    public class MenuFloat : MenuValue, IMenuValue<float>
    {
        #region Fields

        private float value, dvalue;

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

                if (Math.Abs(this.value - value) >= float.Epsilon && this.InvokeNotifier(value))
                {
                    this.value = value;
                }
            }
        }

        public float Max => Math.Max(this.From, this.To);

        public float Min => Math.Min(this.From, this.To);

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

            var reverseMode = this.From > this.To;
            var range = this.Max - this.Min;

            width -= this.size.Width;
            base.OnEndScene(point, width);

            if (this.dragging)
            {
                var diff = (int)UserInput.CursorPosition.X - point.X;
                diff = Math.Max(0, Math.Min(barWidth, diff));

                if (reverseMode)
                {
                    diff *= -1;
                }

                this.dvalue = this.From + diff * range / barWidth;
            }

            point.X += width;
            Theme.DrawTextBox(point, this.size, this.GetPrintableStr(this.dvalue), true);
            point.X -= width;
            
            point.Y += this.size.Height;

            var offset = (int)(barWidth / (range / (this.dvalue - this.Min)));

            if (reverseMode)
            {
                offset = barWidth - offset;
            }

            var firstSize = new Size2(offset, barHeight);
            var secondSize = new Size2(barWidth - offset, barHeight);
            
            var barColor = GetContrastingColor(Theme.BackgroundColor);
            Theme.DrawBox(point, firstSize, barColor);
            
            point.X += offset;
            Theme.DrawBox(point, secondSize);
            point.X -= offset;

            var s = new Size2(this.size.Width, barHeight);

            void ShowNum(float num) => Theme.DrawTextBox(point, s, Color.Transparent, $"{num:0.##}", true);

            ShowNum(this.From);
            point.X += barWidth - s.Width;
            ShowNum(this.To);
        }

        private static Color GetContrastingColor(Color c)
        {
            c = new Color(c.R > 127 ? 0 : 255, c.G > 127 ? 0 : 255, c.B > 127 ? 0 : 255);
            c.A /= 3;

            return c;
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

                this.Value = this.dvalue;
            }
        }

        protected override JToken Token
        {
            get => this.value;
            set => this.dvalue = this.value = value.Value<float>();
        }

        #endregion
    }
}