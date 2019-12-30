namespace Surgical.SDK.Rendering
{
    using SharpDX;
    using SharpDX.Direct3D9;

    public static class Picture
    {
        private static readonly Sprite Sprite;

        static Picture()
        {
            Sprite = new Sprite(Render.Device);

            Render.OnDispose += () => Sprite.Dispose();
            Render.OnLostDevice += () => Sprite.OnLostDevice();
            Render.OnResetDevice += () => Sprite.OnResetDevice();
        }

        public static void Draw(Vector2 position, Texture texture, Color? color = null, Vector3? center = null, Rectangle? rectangle = null, float? rotation = null, Vector2? scale = null)
        {
            var c = color ?? Color.White;
            var positionRef = new Vector3(position, 0);

            if (center.HasValue)
            {
                positionRef += center.Value;
            }

            Sprite.Begin(SpriteFlags.AlphaBlend);

            if (!rotation.HasValue || !scale.HasValue)
            {
                Sprite.Draw(texture, c, rectangle, center, positionRef);
            }
            else
            {
                var transform = Sprite.Transform;

                Sprite.Transform *= Matrix.Scaling(new Vector3(scale.Value, 0)) * Matrix.RotationZ(rotation.Value) * Matrix.Translation(positionRef);
                Sprite.Draw(texture, c, rectangle, center);
                Sprite.Transform = transform;
            }

            Sprite.End();
        }
    }
}