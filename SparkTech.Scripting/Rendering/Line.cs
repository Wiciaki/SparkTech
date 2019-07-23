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

namespace SparkTech.Rendering
{
    #region Using Directives

    using System;

    using SharpDX;
    using SharpDX.Mathematics.Interop;

    #endregion

    /// <summary>
    ///     Class Line
    /// </summary>
    public static class Line
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="Line" /> class.
        /// </summary>
        static Line()
        {
            Initialize();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the cached line.
        /// </summary>
        /// <value>
        ///     The cached line.
        /// </value>
        private static SharpDX.Direct3D9.Line CachedLine { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is drawing.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is drawing; otherwise, <c>false</c>.
        /// </value>
        private static bool IsDrawing { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void Render(Color color, float thickness, bool antiAlias, params Vector2[] screenPositions)
        {
            Render(color, thickness, antiAlias, Array.ConvertAll(screenPositions, v => new RawVector2(v.X, v.Y)));
        }

        /// <summary>
        ///     Renders a line.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="antiAlias">if set to <c>true</c> [anti alias].</param>
        /// <param name="screenPositions">The screen positions.</param>
        public static void Render(Color color, float thickness, bool antiAlias, params RawVector2[] screenPositions)
        {
            // Make sure we can draw the line
            if (screenPositions.Length < 2 || thickness < float.Epsilon)
            {
                return;
            }

            // Check if the line is still being drawn
            if (!IsDrawing)
            {
                // If not, set the line parameters
                CachedLine.Width = thickness;
                CachedLine.Antialias = antiAlias;

                // And begin the drawing process
                CachedLine.Begin();
                IsDrawing = true;
            }

            // Draw the cached line
            CachedLine.Draw(screenPositions, color);

            // Finish drawing and release control of the object
            IsDrawing = false;
            CachedLine.End();
        }

        /// <summary>
        ///     Renders a line.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="antiAlias">if set to <c>true</c> [anti alias].</param>
        /// <param name="worldPositions">The world positions.</param>
        public static void Render(Color color, float thickness, bool antiAlias, params Vector3[] worldPositions)
        {
            // Convert the world space positions to screen space positions
            var screenPositions = Array.ConvertAll(worldPositions, Renderer.WorldToScreen);

            // Draw the screen space positions
            Render(color, thickness, antiAlias, screenPositions);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        private static void Dispose()
        {
            CachedLine?.Dispose();
            CachedLine = null;
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        private static void Initialize()
        {
            // Set the cached line so we don't have to intiialize it every frame
            CachedLine = new SharpDX.Direct3D9.Line(Renderer.Direct3DDevice);

            // Listen to events
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => Dispose();
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => Dispose();

            Renderer.OnReset += args =>
            {
                Unload();
                Reset();
            };
        }

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        private static void Reset()
        {
            CachedLine?.OnResetDevice();
        }

        /// <summary>
        ///     Unloads this instance.
        /// </summary>
        private static void Unload()
        {
            CachedLine?.OnLostDevice();
        }

        #endregion
    }
}