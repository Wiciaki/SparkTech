//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  Line.cs is a part of SparkTech
//
//  SparkTech is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  SparkTech is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  You should have received a copy of the GNU General Public License
//  along with SparkTech. If not, see <http://www.gnu.org/licenses/>.
//
//  -------------------------------------------------------------------

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
            Line = new Line(Render.Direct3DDevice) { Antialias = true };

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
            // Make sure we can draw the line
            if (vertices.Length < 2 || thickness <= 0f)
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