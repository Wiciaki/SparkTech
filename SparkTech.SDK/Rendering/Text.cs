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

namespace SparkTech.SDK.Rendering
{
    #region Using Directives

    using System;

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

        /// <summary>
        ///     Initializes static members of the <see cref="Text" /> class.
        /// </summary>
        static Text()
        {
            // Intialize the default font now so we don't have to intialize it with every frame
            Font = new Font(
                Renderer.Direct3DDevice,
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

            // Listen to events            
            Renderer.OnReset += args =>
            {
                Font.OnLostDevice();
                Font.OnResetDevice();
            };
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Renders text.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="color">The color.</param>
        /// <param name="worldPositions">The world positions.</param>
        public static void Render(string content, Color color, params Vector3[] worldPositions)
        {
            Render(content, color, Array.ConvertAll(worldPositions, Renderer.WorldToScreen));
        }

        /// <summary>
        ///     Renders text.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="color">The color.</param>
        /// <param name="screenPositions">The screen positions.</param>
        public static void Render(string content, Color color, params Vector2[] screenPositions)
        {
            foreach (var screenPosition in screenPositions)
            {
                Font.DrawText(null, content, (int)screenPosition.X, (int)screenPosition.Y, color);
            }
        }

        /// <summary>
        ///     Renders text.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="color">The color.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="rectangles">The rectangles.</param>
        public static void Render(string content, Color color, FontDrawFlags flags, params Rectangle[] rectangles)
        {
            foreach (var rectangle in rectangles)
            {
                Font.DrawText(null, content, rectangle, flags, color);
            }
        }

        /// <summary>
        ///     Renders text.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="color">The color.</param>
        /// <param name="font">The font.</param>
        /// <param name="worldPositions">The world positions.</param>
        public static void Render(string content, Color color, Font font, params Vector3[] worldPositions)
        {
            Render(content, color, font, Array.ConvertAll(worldPositions, Renderer.WorldToScreen));
        }

        /// <summary>
        ///     Renders text.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="color">The color.</param>
        /// <param name="font">The font.</param>
        /// <param name="screenPositions">The screen positions.</param>
        public static void Render(string content, Color color, Font font, params Vector2[] screenPositions)
        {
            foreach (var screenPosition in screenPositions)
            {
                font.DrawText(null, content, (int)screenPosition.X, (int)screenPosition.Y, color);
            }
        }

        /// <summary>
        ///     Renders text.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="color">The color.</param>
        /// <param name="font">The font.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="rectangles">The rectangles.</param>
        public static void Render(
            string content,
            Color color,
            Font font,
            FontDrawFlags flags,
            params Rectangle[] rectangles)
        {
            foreach (var rectangle in rectangles)
            {
                font.DrawText(null, content, rectangle, flags, color);
            }
        }

        #endregion
    }
}