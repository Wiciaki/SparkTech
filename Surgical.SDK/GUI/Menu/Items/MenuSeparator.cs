namespace Surgical.SDK.GUI.Menu
{
    using SharpDX;

    public class MenuSeparator : MenuItem
    {
        private Size2 size;

        private readonly int heightMultiplier;

        public MenuSeparator(string id, int heightMultiplier = 1) : base(id)
        {
            this.heightMultiplier = heightMultiplier;
        }

        protected override Size2 GetSize()
        {
            return this.size = new Size2(0, this.heightMultiplier * Theme.MinItemHeight);
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            Theme.DrawBox(point, new Size2(width, this.size.Height), Theme.BackgroundColor);
        }
    }
}