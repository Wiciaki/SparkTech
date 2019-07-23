namespace SparkTech.Utils
{
    using System;
    using System.Drawing;

    using Entropy.ToolKit;

    using SharpDX;

    using Point = System.Drawing.Point;

    public static class Extensions
    {
        public static bool IsLeftClick(this WindowMessage message)
        {
            return message == WindowMessage.LBUTTONDOWN || message == WindowMessage.LBUTTONDBLCLK;
        }

        public static bool IsInside(this Point mouse, Point point, Size size)
        {
            return mouse.X >= point.X && mouse.Y >= point.Y && mouse.X < point.X + size.Width && mouse.Y < point.Y + size.Height;
        }

        public static bool IsInside(this Vector2 mouse, Point point, Size size)
        {
            return mouse.X >= point.X && mouse.Y >= point.Y && mouse.X < point.X + size.Width && mouse.Y < point.Y + size.Height;
        }

        public static Point ToPoint(this Vector2 v)
        {
            return new Point((int)v.X, (int)v.Y);
        }

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        internal static void SafeInvoke(this Action @event, string eventName)
        {
            if (@event == null)
            {
                return;
            }

            var errorMessage = $"Couldn't invoke a \"{eventName}\" listener.";

            Array.ForEach(@event.GetInvocationList(), d => ((Action)d).TryExecute(errorMessage));
        }

        internal static void TryExecute(this Action action, string errorMessage)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ex.LogException(errorMessage);
            }
        }
    }
}
