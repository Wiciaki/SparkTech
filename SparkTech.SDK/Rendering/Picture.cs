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

namespace SparkTech.SDK.Rendering
{
    using SharpDX;
    using SharpDX.Direct3D9;

    public static class Picture
    {
        private static readonly Sprite Sprite;

        static Picture()
        {
            Sprite = new Sprite(Render.Direct3DDevice);

            Render.OnDispose += Sprite.Dispose;
            Render.OnLostDevice += Sprite.OnLostDevice;
            Render.OnResetDevice += Sprite.OnResetDevice;
        }

        public static void Draw(Vector2 position, Texture texture, Color? color = null, Vector3? center = null, Rectangle? rectangle = null, float? rotation = null, Vector2? scale = null)
        {
            if (!color.HasValue)
            {
                color = Color.Transparent;
            }

            var positionRef = new Vector3(position, 0);

            if (center.HasValue)
            {
                positionRef += center.Value;
            }

            Sprite.Begin();

            if (!rotation.HasValue || !scale.HasValue)
            {
                Sprite.Draw(texture, color.Value, rectangle, center, positionRef);
            }
            else
            {
                // Save the old tranformation for restoring later
                var oldTransform = Sprite.Transform;

                // Transform the sprite and draw it
                Sprite.Transform *= Matrix.Scaling(new Vector3(scale.Value, 0)) * Matrix.RotationZ(rotation.Value) * Matrix.Translation(positionRef);

                Sprite.Draw(texture, color.Value, rectangle, center);

                // Restore the previous transform
                Sprite.Transform = oldTransform;
            }

            // Finish the drawing sequence and release control of the object
            Sprite.End();
        }
    }
}