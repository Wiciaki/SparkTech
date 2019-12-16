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

namespace Surgical.SDK.Rendering
{
    using SharpDX;
    using SharpDX.Direct3D9;

    using Surgical.SDK.API;

    public static class Picture
    {
        private static readonly Sprite Sprite;

        static Picture()
        {
            if (!Platform.HasRender)
            {
                return;
            }

            Sprite = new Sprite(Render.Device);

            Render.OnDispose += () => Sprite.Dispose();
            Render.OnLostDevice += () => Sprite.OnLostDevice();
            Render.OnResetDevice += () => Sprite.OnResetDevice();
        }

        public static void Draw(Vector2 position, Texture texture, Color? color = null, Vector3? center = null, Rectangle? rectangle = null, float? rotation = null, Vector2? scale = null)
        {
            if (!Platform.HasRender)
            {
                return;
            }

            var c = color ?? Color.White;
            var positionRef = new Vector3(position, 0);

            if (center.HasValue)
            {
                positionRef += center.Value;
            }

            Sprite.Begin(SpriteFlags.AlphaBlend);

            if (!rotation.HasValue || !scale.HasValue)
            {
                Sprite.Draw(texture, c, rectangle, center, positionRef);
            }
            else
            {
                var transform = Sprite.Transform;

                Sprite.Transform *= Matrix.Scaling(new Vector3(scale.Value, 0)) * Matrix.RotationZ(rotation.Value) * Matrix.Translation(positionRef);

                Sprite.Draw(texture, c, rectangle, center);

                Sprite.Transform = transform;
            }

            Sprite.End();
        }
    }
}