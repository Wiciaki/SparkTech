namespace SparkTech.SDK.GUI.Menu.Items
{
    using System.Drawing;

    public class MenuSeparator : MenuItem
    {
        private readonly Size size;

        public MenuSeparator(string id) : base(id)
        {

        }

        public MenuSeparator(string id, Size size) : this(id)
        {
            this.size = size;
        }

        protected override Size GetSize()
        {
            return this.size.Height != 0 ? this.size : new Size(28, 28);
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            Theme.DrawBox(Theme.BackgroundColor, point, new Size(width, this.size.Height));
        }
    }
}