namespace SparkTech.SDK.GUI.Menu
{
    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.Rendering;

    public class MenuImage : MenuItem, IMenuValue<Texture>
    {
        public MenuImage(string id) : base(id)
        {

        }

        private Texture texture;

        private Size2 size;

        /*
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
        }*/

        public Texture Value
        {
            get => this.texture;
            set
            {
                if (this.texture != value)
                {
                    this.UpdateTexture(value, value?.GetSize() ?? Size2.Empty);
                }
            }
        }

        private void UpdateTexture(Texture t, Size2 s)
        {
            if (!this.UpdateValue(t))
            {
                return;
            }

            this.texture = t;
            this.size = s;

            this.UpdateSize();
        }

        protected override Size2 GetSize()
        {
            return this.size;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            var s = new Size2((width - this.size.Width) / 2, this.size.Height);

            Theme.DrawBox(point, s, Theme.BackgroundColor);
            point.X += s.Width;

            Image.Draw(point, this.texture, Theme.BackgroundColor);

            point.X += this.size.Width;
            Theme.DrawBox(point, this.size, Theme.BackgroundColor);
        }
    }
}