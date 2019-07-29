//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  Operations.cs is a part of SparkTech
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

namespace SparkTech.SDK.Util.Vector
{
    #region Using Directives

    using SharpDX;

    using SharpDXColor = SharpDX.Color;
    using SystemColor = System.Drawing.Color;

    #endregion

    public static class Operations
    {
        #region Public Methods and Operators

        public static Vector3 To3D(this Vector2 vector2)
        {
            return new Vector3(vector2.X, NavGrid.GetHeightForPosition(vector2), vector2.Y);
        }

        public static Vector2 To2D(this Vector3 vector3)
        {
            return new Vector2(vector3.X, vector3.Z);
        }

        public static Vector3 ToFlat3D(this Vector2 vector2)
        {
            return new Vector3(vector2.X, 0, vector2.Y);
        }

        public static Vector3 ToFlat3D(this Vector3 vector3)
        {
            return new Vector3(vector3.X, 0, vector3.Y);
        }

        public static SharpDXColor ToSharpDXColor(this SystemColor color)
        {
            return new SharpDXColor(color.R, color.G, color.B, color.A);
        }

        public static SystemColor ToSystemColor(this SharpDXColor color)
        {
            return SystemColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        #endregion
    }
}