namespace SparkTech.SDK.UI.Menu.Values
{
    using System.Drawing;

    public class MenuSeparator : MenuComponent
    {
        private readonly Size? suppliedSize;

        public MenuSeparator(string id) : base(id)
        {

        }

        public MenuSeparator(string id, Size suppliedSize) : this(id)
        {
            this.suppliedSize = suppliedSize;
        }

        protected override Size GetSize()
        {
            return this.suppliedSize ?? Theme.MeasureSize("This is a separator");
        }

        protected internal override void OnEndScene(Point point, Size size)
        {
            Theme.Draw(new DrawData(point, size));
        }
    }
}