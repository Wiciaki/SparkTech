namespace SparkTech.SDK.GUI.Menu.Items
{
    using System.Drawing;

    using SharpDX.Direct3D9;

    using SparkTech.SDK.Misc;
    using SparkTech.SDK.Rendering;

    public class MenuImage : MenuItem, IMenuValue<Texture>
    {
        public MenuImage(string id) : base(id)
        {

        }

        private Texture texture;

        private Size size;

        public Bitmap Bitmap
        {
            set
            {
                if (value == null)
                {
                    this.Texture = null;
                    return;
                }

                this.texture = value.ToTexture();
                this.size = value.Size;
                this.UpdateSize();
            }
        }

        public Texture Texture
        {
            get => this.texture;
            set
            {
                this.texture = value;
                this.size = this.texture.GetSize();
                this.UpdateSize();
            }
        }

        protected override Size GetSize()
        {
            return this.size;
        }

        protected internal override void OnEndScene(Point point, int groupWidth)
        {
            Picture.Draw(point.ToVector2(), this.texture, Theme.BackgroundColor);
        }

        Texture IMenuValue<Texture>.Value
        {
            get => this.Texture;
            set => this.Texture = value;
        }
    }
}