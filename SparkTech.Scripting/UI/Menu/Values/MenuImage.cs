namespace SparkTech.UI.Menu.Values
{
    using System.Drawing;

    using SparkTech.Rendering;
    using SparkTech.Utils;

    public class MenuImage : MenuComponent
    {
        public Image Image { get; private set; }

        private SharpDX.Direct3D9.Texture texture;

        public MenuImage(string id) : base(id)
        {

        }

        public virtual MenuImage SetImage(Image image)
        {
            this.Image = image;

            this.texture = image?.ToTexture();

            this.UpdateSize();

            return this;
        }

        protected override Size GetSize() => this.Image?.Size ?? default;

        protected internal override void OnEndScene(Point point, Size size)
        {
            if (size == default)
            {
                return;
            }

            Theme.Draw(new DrawData(point, size));

            var diff = size.Width - this.Image.Size.Width;

            if (diff > 0)
            {
                point.X += diff / 2;
            }

            Texture.Render(point.ToVector2(), this.texture);
        }
    }
}
