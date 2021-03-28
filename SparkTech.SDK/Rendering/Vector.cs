namespace SparkTech.SDK.Rendering
{
    #region Using Directives

    using SharpDX;
    using SharpDX.Direct3D9;

    #endregion

    /// <summary>
    ///     Class Vector
    /// </summary>
    public static class Vector
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="Vector" /> class.
        /// </summary>
        static Vector()
        {
            Line = new Line(Render.Device) { Antialias = true };

            Render.OnDispose += Line.Dispose;
            Render.OnLostDevice += Line.OnLostDevice;
            Render.OnResetDevice += Line.OnResetDevice;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the cached line.
        /// </summary>
        /// <value>
        ///     The cached line.
        /// </value>
        private static readonly Line Line;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Renders a line.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="vertices">The screen positions.</param>
        public static void Draw(Color color, float thickness, params Vector2[] vertices)
        {
            if (vertices.Length < 2 || thickness < float.Epsilon)
            {
                return;
            }

            Line.Width = thickness;

            Line.Begin();
            Line.Draw(vertices, color);
            Line.End();
        }

        #endregion
    }
}