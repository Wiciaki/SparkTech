namespace SparkTech.SDK.GUI.Menu
{
    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.Rendering;

    public class MenuTexture : MenuItem, IMenuValue<Texture>
    {
        public MenuTexture(string id) : base(id)
        {

        }

        private Texture texture;

        private Size2 size;

        public Texture Texture
        {
            get => this.GetValue<Texture>();
            set => this.SetValue(value);
        }

        Texture IMenuValue<Texture>.Value
        {
            get => this.texture;
            set
            {
                if (this.texture == value || !this.UpdateValue(value))
                {
                    return;
                }

                var desc = value.GetLevelDescription(0);
                var s = new Size2(desc.Width, desc.Height);

                this.texture = value;
                this.size = s;

                this.UpdateSize();

            }
        }

        protected override Size2 GetSize()
        {
            return this.size;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            Theme.DrawBox(point, new Size2(width, this.size.Height), Theme.BackgroundColor);
            Image.Draw(point, this.texture);
        }
    }
}