//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  Texture.cs is a part of SparkTech
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
    using System.Drawing;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = SharpDX.Color;
    using Rectangle = SharpDX.Rectangle;

    #endregion

    /// <summary>
    ///     Class Texture
    /// </summary>
    public static class Texture
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="Texture" /> class.
        /// </summary>
        static Texture()
        {
            Initialize();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the cached sprite.
        /// </summary>
        /// <value>
        ///     The cached sprite.
        /// </value>
        private static SharpDX.Direct3D9.Sprite CachedSprite { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is drawing.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is drawing; otherwise, <c>false</c>.
        /// </value>
        private static bool IsDrawing { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Renders a sprite.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="backgroundColor">The color.</param>
        /// <param name="center">The center.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The scale.</param>
        public static void Render(
            Vector2 position,
            SharpDX.Direct3D9.Texture texture,
            Color backgroundColor = default,
            Vector3? center = null,
            Rectangle? rectangle = null,
            float? rotation = null,
            Vector2? scale = null)
        {
            // Make sure we can render
            if (CachedSprite.IsDisposed)
            {
                return;
            }

            // Check if the sprite is currently being drawn
            if (!IsDrawing)
            {
                // If not, begin the drawing process
                CachedSprite.Begin();
                IsDrawing = true;
            }

            // If there is no need to apply and properties, draw the sprite
            if (rotation == null || scale == null)
            {
                CachedSprite.Draw(
                    texture,
                    backgroundColor,
                    rectangle,
                    center,
                    new Vector3(position, 0) + (center ?? Vector3.Zero));

                // Finish the drawing sequence and release control of the object
                IsDrawing = false;
                CachedSprite.End();

                // Return from the function
                return;
            }

            // Save the old tranformation for restoring later
            var oldTransform = CachedSprite.Transform;

            // Transform the sprite and draw it
            CachedSprite.Transform *= Matrix.Scaling(new Vector3((Vector2)scale, 0))
                                      * Matrix.RotationZ((float)rotation) * Matrix.Translation(
                                          new Vector3(position, 0) + (center ?? Vector3.Zero));
            CachedSprite.Draw(texture, backgroundColor, rectangle, center);

            // Restore the previous transform
            CachedSprite.Transform = oldTransform;

            // Finish the drawing sequence and release control of the object
            IsDrawing = false;
            CachedSprite.End();
        }

        public static SharpDX.Direct3D9.Texture ToTexture(this Image image, float width = -1, float height = -1)
        {
            var size = (int)Math.Min(width, height);

            return SharpDX.Direct3D9.Texture.FromMemory(
                Renderer.Direct3DDevice,
                (byte[])new ImageConverter().ConvertTo(image, typeof(byte[])),
                width.Equals(-1) ? image.Width : size,
                height.Equals(-1) ? image.Height : size,
                0,
                Usage.None,
                Format.A1,
                Pool.Managed,
                Filter.Default,
                Filter.Default,
                0);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        private static void Dispose()
        {
            CachedSprite?.Dispose();
            CachedSprite = null;
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        private static void Initialize()
        {
            // Intialize the sprite now so we don't have to intialize it with every frame
            CachedSprite = new SharpDX.Direct3D9.Sprite(Renderer.Direct3DDevice);

            // Listen to events
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => Dispose();
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => Dispose();

            Renderer.OnReset += args =>
            {
                Reset();
            };

            Renderer.OnPostReset += args => Unload();
        }

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        private static void Reset()
        {
            CachedSprite?.OnResetDevice();
        }

        /// <summary>
        ///     Unloads this instance.
        /// </summary>
        private static void Unload()
        {
            CachedSprite?.OnLostDevice();
        }

        #endregion
    }
}