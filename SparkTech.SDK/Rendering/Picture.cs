namespace SparkTech.SDK.Rendering
{
    using SharpDX;
    using SharpDX.Direct3D9;

    public static class Picture
    {
        private static readonly Sprite Sprite;

        static Picture()
        {
            Sprite = new Sprite(Render.Device);

            Render.OnDispose += Sprite.Dispose;
            Render.OnLostDevice += Sprite.OnLostDevice;
            Render.OnResetDevice += Sprite.OnResetDevice;
        }

        public static void Draw(Vector2 position, Texture texture, Color? color = null, Vector3? center = null, Rectangle? rectangle = null, float? rotation = null, Vector2? scale = null)
        {
            var col = color ?? Color.White;
            var pos = new Vector3(position, 0);

            if (center.HasValue)
            {
                pos += center.Value;
            }

            Sprite.Begin(SpriteFlags.AlphaBlend);

            if (!rotation.HasValue || !scale.HasValue)
            {
                Sprite.Draw(texture, col, rectangle, center, pos);
            }
            else
            {
                var transform = Sprite.Transform;

                Sprite.Transform *= Matrix.Scaling(new Vector3(scale.Value, 0)) * Matrix.RotationZ(rotation.Value) * Matrix.Translation(pos);
                Sprite.Draw(texture, col, rectangle, center);
                Sprite.Transform = transform;
            }

            Sprite.End();
        }
    }
}