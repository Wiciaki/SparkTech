namespace Surgical.SDK.Rendering
{
    #region Using Directives

    using SharpDX;
    using SharpDX.Direct3D9;

    #endregion

    public static class Text
    {
        #region Static Fields

        private static readonly Font Font;

        #endregion

        #region Constructors and Destructors

        static Text()
        {
            Font = new Font(
                Render.Device,
                20,
                0,
                FontWeight.Normal,
                0,
                false,
                FontCharacterSet.Default,
                FontPrecision.TrueType,
                FontQuality.ClearType,
                FontPitchAndFamily.Default | FontPitchAndFamily.DontCare,
                "Arial");

            Render.OnResetDevice += Font.OnResetDevice;
            Render.OnLostDevice += Font.OnLostDevice;
            Render.OnDispose += Font.Dispose;
        }

        #endregion

        #region Public Methods and Operators

        public static void Draw(string text, Color color, Point point)
        {
            Font.DrawText(null, text, point.X, point.Y, color);
        }

        public static void Draw(string text, Color color, FontDrawFlags flags, Rectangle rectangle)
        {
            Font.DrawText(null, text, rectangle, flags, color);
        }

        #endregion
    }
}