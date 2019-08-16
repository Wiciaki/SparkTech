namespace SparkTech.SDK.GUI.Menu
{
    using System.Drawing;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Rendering;

    public class MenuText : MenuItem
    {
        public MenuText(string id) : base(id)
        {

        }

        private const string HelpBoxText = "[?]";

        private string text, helpText;

        private Size textSize, helpSize, helpTextSize;

        public string Text
        {
            get => this.text ?? "PLACEHOLDER";
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

        protected internal override void SetTranslations(JObject o)
        {
            this.Text = o["text"].Value<string>();

            this.HelpText = o["helpText"]?.Value<string>();
        }

        protected override Size GetSize()
        {
            this.UpdateHelpTextSize();

            this.textSize = Theme.MeasureText(this.Text);

            var hSize = Theme.MeasureText(HelpBoxText);
            hSize.Width = this.textSize.Width;

            this.helpSize = hSize;

            return new Size(this.helpSize.Width + this.textSize.Width, this.textSize.Height);
        }

        private void UpdateHelpTextSize()
        {
            this.helpTextSize = this.HelpText != null ? Theme.MeasureText(this.HelpText) : default;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            var size = new Size(width - this.helpSize.Width, this.textSize.Height);

            Theme.DrawTextBox(point, size, this.Text);

            if (this.HelpText == null)
            {
                return;
            }

            point.X += size.Width;

            Theme.DrawTextBox(point, this.helpSize, HelpBoxText);

            if (!Menu.IsCursorInside(point, this.helpSize))
            {
                return;
            }

            // todo maybe non centered?
            var res = Render.Resolution();

            point.X = (res.Width - this.helpTextSize.Width) / 2;
            point.Y = (res.Height - this.helpTextSize.Height) / 2;

            Theme.DrawTextBox(point, this.helpTextSize, this.HelpText);
            Theme.DrawBorders(point, this.helpTextSize);
        }
    }
}