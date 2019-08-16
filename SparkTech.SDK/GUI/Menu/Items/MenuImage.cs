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
                    this.Value = null;
                    return;
                }

                this.UpdateTexture(value.ToTexture(), value.Size);
            }
        }

        public Texture Value
        {
            get => this.texture;
            set
            {
                if (this.texture != value)
                {
                    this.UpdateTexture(value, value?.GetSize() ?? Size.Empty);
                }
            }
        }

        private void UpdateTexture(Texture t, Size s)
        {
            if (!this.UpdateValue(t))
            {
                return;
            }

            this.texture = t;
            this.size = s;

            this.UpdateSize();
        }

        protected override Size GetSize()
        {
            return this.size;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            // todo background before and after
            Picture.Draw(point.ToVector2(), this.texture, Theme.BackgroundColor);
        }
    }
}