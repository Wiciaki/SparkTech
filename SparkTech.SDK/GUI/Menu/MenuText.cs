namespace SparkTech.SDK.GUI.Menu
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Rendering;

    public class MenuText : MenuItem
    {
        public MenuText(string id) : base(id)
        {

        }

        private static bool arrows;

        internal static void SetArrows(bool b)
        {
            arrows = b;
        }

        private const string HelpBoxText = "[?]";

        private const string Arrow = "➜";

        private string text, helpText;

        private Size2 textSize, helpSize, helpTextSize;

        private static Size2 arrowSize;

        public string Text
        {
            get => this.text ?? this.Id;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = null;
                }

                if (this.text == value)
                {
                    return;
                }

                this.text = value;
                this.UpdateSize();
            }
        }

        public string HelpText
        {
            get => this.helpText;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = null;
                }

                if (this.helpText == value)
                {
                    return;
                }

                this.helpText = value;
                this.UpdateHelpTextSize();
            }
        }

        protected internal override void SetTranslations(Translations t)
        {
            this.HelpText = t.GetString("helpText");

            this.Text = t.GetString("text");
        }

        protected override Size2 GetSize()
        {
            this.UpdateHelpTextSize();

            this.textSize = Theme.MeasureText(this.Text);

            if (this.HelpText != null)
            {
                this.helpSize = new Size2(Theme.MeasureText(HelpBoxText).Width, this.textSize.Height);
            }

            var size = new Size2(this.textSize.Width + this.helpSize.Width, this.textSize.Height);

            var width = Theme.MeasureText("This is enough").Width;

            if (width > size.Width)
            {
                size.Width = width;
            }

            return size;
        }

        protected static Size2 AddButton(Size2 size, out Size2 buttonSize, string minWidth = null)
        {
            var width = Math.Min(Theme.MinItemHeight, size.Height);

            if (minWidth != null)
            {
                var measuredWidth = Theme.MeasureText(minWidth).Width;

                if (measuredWidth > width)
                {
                    width = measuredWidth;
                }
            }

            size.Width += width;

            buttonSize = new Size2(width, size.Height);
            return size;
        }

        protected static int ArrowWidth => arrows ? arrowSize.Width : 0;

        protected static void AddArrow(Point point)
        {
            if (arrows)
            {
                Theme.DrawTextBox(point, arrowSize, "➜", true, Color.Transparent);
            }
        }

        internal static void UpdateArrowSize()
        {
            arrowSize = new Size2(Theme.MeasureText(Arrow).Width, Theme.MinItemHeight);
        }

        private void UpdateHelpTextSize()
        {
            this.helpTextSize = this.HelpText != null ? Theme.MeasureText(this.HelpText) : default;
        }

        protected Color BackgroundColor
        {
            get
            {
                var color = Theme.BackgroundColor;

                if (this is IExpandable expandable && expandable.IsExpanded)
                {
                    color.A = byte.MaxValue;
                }

                return color;
            }
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            var size = new Size2(width - this.helpSize.Width, this.textSize.Height);

            Theme.DrawTextBox(point, size, this.Text, false, this.BackgroundColor);

            if (this.HelpText == null)
            {
                return;
            }

            point.X += size.Width;

            var cursorInside = Menu.IsCursorInside(point, this.helpSize);

            Theme.DrawTextBox(point, this.helpSize, HelpBoxText, true, this.BackgroundColor);

            if (!cursorInside)
            {
                return;
            }

            // todo maybe non centered?
            var res = Render.Resolution();

            point.X = (res.Width - this.helpTextSize.Width) / 2;
            point.Y = (res.Height - this.helpTextSize.Height) / 2;

            Theme.DrawTextBox(point, this.helpTextSize, this.HelpText, true);
        }
    }
}