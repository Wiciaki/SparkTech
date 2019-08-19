namespace SparkTech.SDK.GUI.Menu.Items
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Rendering;

    // TODO: revisit it when drawings are there and make MenuFLoat
    public class MenuInt : MenuValue, IMenuValue<int>
    {
        #region Fields

        private int value, draggingVal;

        private bool dragging;

        #endregion

        #region Constructors and Destructors

        public MenuInt(string id, int from, int to, int defaultValue) : base(id, defaultValue)
        {
            this.From = from;

            this.To = to;
        }

        #endregion

        #region Public Properties

        public readonly int From;

        public readonly int To;

        public int Value
        {
            get => this.value;
            set
            {
                if (value == this.value)
                {
                    return;
                }

                // is in bounds?

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

            this.size = Theme.MeasureText($"[{this.From}]");

            s.Width += this.size.Width;

            return s;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            var barWidth = width;
            var displayNum = this.Value;
            var range = this.To - this.From;

            width -= this.size.Width;

            base.OnEndScene(point, width);

            if (this.dragging)
            {
                var diff = GameInterface.CursorPosition().X - point.X;

                if (diff <= 0)
                {
                    displayNum = this.From;
                }
                else if (diff >= barWidth)
                {
                    displayNum = this.To;
                }
                else
                {
                    displayNum = this.From + (int)((float)diff * range / barWidth);
                }

                this.draggingVal = displayNum;
            }

            point.X += width;
            Theme.DrawTextBox(point, this.size, $"[{displayNum}]");
            point.X -= width;

            point.Y += this.size.Height;
            Theme.DrawBox(point, new Size2(barWidth, SliderHeight), Theme.BackgroundColor);

            var offset = (int)(barWidth / ((float)range / (this.Value - this.From)));

            var color = Color.White;

            if (!this.dragging)
            {
                color.A = 150;
            }

            Vector.Draw(color, SliderHeight, point, new Point(point.X + offset, point.Y));
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            if (!this.dragging)
            {
                point.Y += this.size.Height;

                if (!Menu.IsCursorInside(point, new Size2(width, SliderHeight)))
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
            get => this.value;
            set => this.value = value.Value<int>();
        }

        #endregion
    }
}