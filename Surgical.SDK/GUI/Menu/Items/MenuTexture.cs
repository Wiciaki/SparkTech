namespace Surgical.SDK.GUI.Menu
{
    using System;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Surgical.SDK.API;
    using Surgical.SDK.Rendering;

    public class MenuTexture : MenuItem
    {
        private readonly Texture texture;

        private readonly Size2 size;

        public MenuTexture(string id, Texture texture) : base(id)
        {
            if (!Platform.HasRender)
            {
                return;
            }

            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            var description = texture.GetLevelDescription(0);

            this.texture = texture;
            this.size = new Size2(description.Width, description.Height);
        }

        protected override Size2 GetSize() => this.size;

        protected internal override void OnEndScene(Point point, int width)
        {
            Theme.DrawBox(point, new Size2(width, this.size.Height), Theme.BackgroundColor);

            point.X += (width - this.size.Width) / 2;

            Picture.Draw(point, this.texture);
        }
    }
}