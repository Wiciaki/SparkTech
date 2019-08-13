namespace SparkTech.SDK.GUI.Menu
{
    using System;
    using System.Drawing;

    using SparkTech.SDK.Rendering;

    public class MenuText : MenuItem
    {
        public MenuText(string id) : base(id)
        {

        }

        private const string HelpBoxText = "[?]";

        private string text;

        private string helpText;

        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                this.UpdateSize();
            }
        }

        public string HelpText
        {
            get => this.helpText;
            set
            {
                this.helpText = value;
                this.UpdateHelpTextBoxSize();
            }
        }

        protected Size TextSize { get; private set; }

        protected Size HelpBoxSize { get; private set; }

        protected Size HelpTextBoxSize { get; private set; }

        protected override Size GetSize()
        {
            var size = this.TextSize;

            if (this.HasHelpText())
            {
                size.Width += this.HelpBoxSize.Width;
            }

            return size;
        }

        private bool HasHelpText()
        {
            return !string.IsNullOrWhiteSpace(this.HelpText);
        }

        protected virtual void UpdateTextBasedSize()
        {

        }

        protected internal sealed override void UpdateSize()
        {
            this.UpdateHelpTextBoxSize();

            var helpBoxSize = Theme.MeasureText(HelpBoxText);
            var textSize = Theme.MeasureText(this.Text);
            
            helpBoxSize.Height = textSize.Height = Math.Max(helpBoxSize.Height, textSize.Height);

            this.HelpBoxSize = helpBoxSize;
            this.TextSize = textSize;

            this.UpdateTextBasedSize();

            base.UpdateSize();
        }

        private void UpdateHelpTextBoxSize()
        {
            this.HelpTextBoxSize = Theme.MeasureText(this.HelpText);
        }

        protected internal override void OnEndScene(Point point, int groupWidth)
        {
            var size = this.TextSize;

            size.Width = groupWidth - (this.Size.Width - this.TextSize.Width);

            Theme.DrawTextBox(this.Text, point, size);

            if (!this.HasHelpText())
            {
                return;
            }

            point.X += size.Width;

            Theme.DrawTextBox(HelpBoxText, point, this.HelpBoxSize);

            if (!point.ToRectangle(this.HelpBoxSize).IsCursorInside())
            {
                return;
            }

            // todo maybe non centered?
            var res = Render.Resolution();

            point.X = (res.Width - this.HelpTextBoxSize.Width) / 2;
            point.Y = (res.Height - this.HelpTextBoxSize.Height) / 2;

            Theme.DrawTextBox(this.HelpText, point, this.HelpTextBoxSize);
        }
    }
}