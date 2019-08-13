namespace SparkTech.SDK.Rendering
{
    using System.Drawing;

    using SharpDX.Direct3D9;

    using Rectangle = SharpDX.Rectangle;

    public static class Extensions
    {
        public static Texture ToTexture(this Bitmap bitmap)
        {
            var buffer = (byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[]));

            // todo verify if call can be shortened
            return Texture.FromMemory(Render.Direct3DDevice, buffer, bitmap.Width, bitmap.Height, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
        }

        public static Size GetSize(this Texture t)
        {
            var desc = t.GetLevelDescription(0);

            return new Size(desc.Width, desc.Height);
        }

        public static Rectangle ToRectangle(this Size size, Point point)
        {
            return new Rectangle(point.X, point.Y, point.X + size.Width, point.Y + size.Height);
        }

        public static Rectangle ToRectangle(this Point point, Size size)
        {
            return size.ToRectangle(point);
        }
    }
}