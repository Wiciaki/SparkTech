//  -------------------------------------------------------------------
//
//  Last updated: 27/09/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  Extensions.cs is a part of SparkTech
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

namespace SparkTech.Utils
{
    #region Using Directives

    using System;
    using System.Runtime.CompilerServices;

    using SharpDX;

    #endregion

    public static class VectorExtensions
    {
        #region Public Methods and Operators

        public static Vector2 Extend(this Vector2 source, Vector2 target, float range)
        {
            return source + target * (target - source).Normalized();
        }

        public static Vector3 Extend(this Vector3 source, Vector3 target, float range)
        {
            return source + target * (target - source).Normalized();
        }

        public static Vector3 Extend(this GameObject source, GameObject target, float range)
        {
            return Extend(source.Position, target.Position, range);
        }

        public static bool IsBuilding(this Vector2 vector2)
        {
            return new NavGridCell(vector2).CollFlags.HasFlag(CollisionFlags.Building);
        }

        public static bool IsBuilding(this Vector3 vector3)
        {
            return new NavGridCell(vector3).CollFlags.HasFlag(CollisionFlags.Building);
        }

        public static bool IsGrass(this Vector2 vector2)
        {
            return new NavGridCell(vector2).CollFlags.HasFlag(CollisionFlags.Grass);
        }

        public static bool IsGrass(this Vector3 vector3)
        {
            return new NavGridCell(vector3).CollFlags.HasFlag(CollisionFlags.Grass);
        }

        public static bool IsWall(this Vector2 vector2)
        {
            return new NavGridCell(vector2).CollFlags.HasFlag(CollisionFlags.Wall | CollisionFlags.Building);
        }

        public static bool IsWall(this Vector3 vector3)
        {
            return new NavGridCell(vector3).CollFlags.HasFlag(CollisionFlags.Wall | CollisionFlags.Building);
        }

        public static Vector2 Midpoint(this Vector2 vec1, Vector2 vec2)
        {
            return new Vector2((vec1.X + vec2.X) / 2, (vec2.Y + vec2.Y) / 2);
        }

        public static Vector3 Midpoint(this Vector3 vec1, Vector3 vec2)
        {
            return new Vector3((vec1.X + vec2.X) / 2, (vec1.Y + vec2.Y) / 2, (vec1.Z + vec2.Z) / 2);
        }

        public static Vector3 Midpoint(this GameObject start, GameObject end)
        {
            return Midpoint(start.Position, end.Position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalized(this Vector2 vector)
        {
            return Vector2.Normalize(vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Normalized(this Vector3 vector)
        {
            return Vector3.Normalize(vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 PerpendicularAntiClockwise(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 PerpendicularAntiClockwise(this Vector3 vector)
        {
            var vec2 = vector.To2D().PerpendicularAntiClockwise();
            return new Vector3(vec2.X, vector.Y, vec2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 PerpendicularClockwise(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static Vector3 PerpendicularClockwise(this Vector3 vector)
        {
            var vec2 = vector.To2D().PerpendicularClockwise();
            return new Vector3(vec2.X, vector.Y, vec2.Y);
        }

        public static float ToDegrees(this float radians)
        {
            return radians * 180f * (float)Math.PI;
        }

        public static float ToRadians(this float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }

        #endregion
    }
}