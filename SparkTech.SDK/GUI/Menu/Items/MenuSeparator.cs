namespace SparkTech.SDK.GUI.Menu.Items
{
    using SharpDX;

    public class MenuSeparator : MenuItem
    {
        private readonly Size2 size;

        public MenuSeparator(string id) : this(id, 28)
        {

        }

        public MenuSeparator(string id, int height) : this(id, new Size2(28, height))
        {

        }

        public MenuSeparator(string id, Size2 size) : base(id)
        {
            this.size = size;
        }

        protected override Size2 GetSize()
        {
            return this.size;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            Theme.DrawBox(point, new Size2(width, this.size.Height), Theme.BackgroundColor);
        }
    }
}