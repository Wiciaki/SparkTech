namespace SparkTech.SDK.Rendering
{
    using SharpDX;
    using SharpDX.Direct3D9;

    public static class Extensions
    {
        public static Rectangle ToRectangle(this Size2 size, Point point)
        {
            return new Rectangle(point.X, point.Y, point.X + size.Width, point.Y + size.Height);
        }

        public static Rectangle ToRectangle(this Point point, Size2 size)
        {
            return size.ToRectangle(point);
        }
    }
}