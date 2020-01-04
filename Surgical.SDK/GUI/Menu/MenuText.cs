namespace Surgical.SDK.GUI.Menu
{
    using System;

    using SharpDX;

    using Surgical.SDK.Rendering;

    public class MenuText : MenuItem
    {
        public MenuText(string id) : base(id)
        {

        }

        private const string HelpBoxText = "[?]";

        private string? text, helpText;

        private Size2 textSize, helpSize, helpTextSize;

        public string Text
        {
            get => this.text ?? this.Id;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || this.text == value)
                {
                    return;
                }

                this.text = value;
                this.UpdateSize();
            }
        }

        public string? HelpText
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
            this.Text = t.GetString("text")!;
        }

        protected override Size2 GetSize()
        {
            this.UpdateHelpTextSize();

            this.textSize = Theme.MeasureText(this.Text);

            var size = new Size2(this.textSize.Width + this.helpSize.Width, this.textSize.Height);
            
            if (size.Height > this.helpSize.Height)
            {
                this.helpSize = new Size2(this.helpSize.Width, size.Height);
            }

            if (Menu.MinItemWidth > size.Width)
            {
                size.Width = Menu.MinItemWidth;
            }

            return size;
        }

        protected static Size2 AddButton(Size2 size, out Size2 buttonSize, string? minWidthText = null)
        {
            var width = Math.Min(Theme.MinItemHeight, size.Height);

            if (minWidthText != null)
            {
                width = Math.Max(width, Theme.MeasureText(minWidthText).Width);
            }

            size.Width += width;

            buttonSize = new Size2(width, size.Height);
            return size;
        }

        private void UpdateHelpTextSize()
        {
            if (this.HelpText == null)
            {
                this.helpTextSize = this.helpSize = default;
            }
            else
            {
                this.helpTextSize = Theme.MeasureText(this.HelpText);
                this.helpSize = Theme.MeasureText(HelpBoxText);
            }
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

            Theme.DrawTextBox(point, size, this.BackgroundColor, this.Text);

            if (this.HelpText == null)
            {
                return;
            }

            point.X += size.Width;

            Theme.DrawTextBox(point, this.helpSize, this.BackgroundColor, HelpBoxText, true);

            if (!Menu.IsCursorInside(point, this.helpSize))
            {
                return;
            }

            point.X = (Render.Resolution.Width - this.helpTextSize.Width) / 2;
            point.Y = this.helpTextSize.Height;

            Theme.DrawTextBox(point, this.helpTextSize, this.HelpText, true);
        }
    }
}