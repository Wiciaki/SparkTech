//  -------------------------------------------------------------------
//
//  Last updated: 28/09/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  Distance.cs is a part of SparkTech
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

    using SharpDX;

    #endregion

    public static class DistanceEx
    {
        #region Public Methods and Operators

        public static float DistanceToPlayer(this Vector3 to)
        {
            return LocalPlayer.Instance.Distance(to);
        }

        public static float DistanceToPlayer(this GameObject to)
        {
            return LocalPlayer.Instance.Distance(to);
        }

        public static float Distance(this Vector2 from, Vector2 to)
        {
            return Vector2.Distance(from, to);
        }

        public static float Distance(this Vector3 from, Vector3 to)
        {
            return Vector3.Distance(from, to);
        }

        public static float Distance(this Vector3 from, GameObject to)
        {
            return Vector3.Distance(from, to.Position);
        }

        public static float Distance(this GameObject from, Vector3 to)
        {
            return Vector3.Distance(from.Position, to);
        }

        public static float Distance(this GameObject from, GameObject to)
        {
            return Vector3.Distance(from.Position, to.Position);
        }

        public static float DistanceSquared(this Vector3 from, Vector3 to)
        {
            return Vector3.DistanceSquared(from, to);
        }

        public static float DistanceSquared(this Vector3 from, GameObject to)
        {
            return Vector3.DistanceSquared(from, to.Position);
        }

        public static float DistanceSquared(this GameObject from, Vector3 to)
        {
            return Vector3.DistanceSquared(from.Position, to);
        }

        public static float DistanceSquared(this GameObject from, GameObject to)
        {
            return Vector3.DistanceSquared(from.Position, to.Position);
        }

        public static bool IsInRange(this Vector3 source, Vector3 target, float range)
        {
            return DistanceSquared(source, target) <= range.Pow();
        }

        public static bool IsInRange(this GameObject source, GameObject target, float range)
        {
            return IsInRange(source.Position, target.Position, range);
        }

        public static bool IsInRange(this GameObject source, Vector3 target, float range)
        {
            return IsInRange(source.Position, target, range);
        }

        public static bool IsInRange(this Vector3 source, GameObject target, float range)
        {
            return IsInRange(source, target.Position, range);
        }

        #endregion
    }
}