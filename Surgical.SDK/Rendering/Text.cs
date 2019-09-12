// 
//  Last updated: 21/08/2017
//  Created: 26/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  Text.cs is a part of SparkTech
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

namespace Surgical.SDK.Rendering
{
    #region Using Directives

    using SharpDX;
    using SharpDX.Direct3D9;

    #endregion

    /// <summary>
    ///     Class Text
    /// </summary>
    public static class Text
    {
        #region Static Fields

        /// <summary>
        ///     The default font used in rendering if no other font is specified
        /// </summary>
        private static readonly Font Font;

        #endregion

        #region Constructors and Destructors

        static Text()
        {
            // Intialize the default font now so we don't have to intialize it with every frame
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

            Render.OnResetDevice += () => Font.OnResetDevice();
            Render.OnLostDevice += () => Font.OnLostDevice();
            Render.OnDispose += () => Font.Dispose();
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