namespace SparkTech.SDK.GUI.Menu
{
    using SharpDX;

    using SparkTech.SDK.Rendering;

    public class MenuText : MenuItem
    {
        public MenuText(string id) : base(id)
        {

        }

        private const string HelpBoxText = "[?]";

        private string text, helpText;

        private Size2 textSize, helpSize, helpTextSize;

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
            this.Text = t.GetString("text");

            this.HelpText = t.GetString("helpText");
        }

        protected override Size2 GetSize()
        {
            this.UpdateHelpTextSize();

            var s = Theme.MeasureText(this.Text);
            this.textSize = s;

            if (this.HelpText != null)
            {
                var hSize = Theme.MeasureText(HelpBoxText);
                hSize.Height = this.textSize.Height;

                this.helpSize = hSize;
            }

            s.Width += this.helpSize.Width;
            return s;
        }

        private void UpdateHelpTextSize()
        {
            this.helpTextSize = this.HelpText != null ? Theme.MeasureText(this.HelpText) : default;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            var size = new Size2(width - this.helpSize.Width, this.textSize.Height);

            Theme.DrawTextBox(point, size, this.Text);

            if (this.HelpText == null)
            {
                return;
            }

            point.X += size.Width;

            Theme.DrawTextBox(point, this.helpSize, HelpBoxText, true);

            if (!Menu.IsCursorInside(point, this.helpSize))
            {
                return;
            }

            // todo maybe non centered?
            var res = Render.Resolution();

            point.X = (res.Width - this.helpTextSize.Width) / 2;
            point.Y = (res.Height - this.helpTextSize.Height) / 2;

            Theme.DrawTextBox(point, this.helpTextSize, this.HelpText, true);
            Theme.DrawBorders(point, this.helpTextSize);
        }
    }
}