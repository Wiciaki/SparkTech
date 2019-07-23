//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 07/08/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  GameObjectComparer.cs is a part of SparkTech
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
    using System.Collections.Generic;

    /// <inheritdoc />
    /// <summary>
    ///     The comparer class, which is responsible for determining whether GameObject instances represent the same underlying
    ///     game object.
    /// </summary>
    public sealed class GameObjectComparer : IEqualityComparer<GameObject>
    {
        #region Explicit Interface Methods

        /// <inheritdoc />
        /// <summary>
        ///     Determines whether the specified <see cref="T:Entropy.GameObject" /> are equal.
        /// </summary>
        /// <param name="x">The left <see cref="T:Entropy.GameObject" /> to compare.</param>
        /// <param name="y">The right <see cref="T:Entropy.GameObject" /> to compare.</param>
        /// <returns>Value indicating whether the GameObjects represented the same underlying game object.</returns>
        public bool Equals(GameObject x, GameObject y)
        {
            return x != null && y != null && x.NetworkID == y.NetworkID;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Obtains HashCode for the specified <see cref="T:Entropy.GameObject" /> instance.
        /// </summary>
        /// <param name="obj">The specified <see cref="T:Entropy.GameObject" /> instance.</param>
        /// <returns>The HashCode for the specified <see cref="T:Entropy.GameObject" /> instance.</returns>
        public int GetHashCode(GameObject obj)
        {
            return (int)obj.NetworkID;
        }

        #endregion
    }
}