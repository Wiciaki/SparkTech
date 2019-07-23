//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 05/08/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  CollisionableObjects.cs is a part of SparkTech
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

namespace SparkTech.Enumerations
{
    using System;

    /// <summary>
    ///     Enum CollisionableObjects
    /// </summary>
    [Flags]
    public enum CollisionableObjects
    {
        /// <summary>
        ///     Minion Collision-able Flag
        /// </summary>
        Minions = 1 << 0,

        /// <summary>
        ///     Hero Collision-able Flag
        /// </summary>
        Heroes = 1 << 1,

        /// <summary>
        ///     Yasuo's Wall Collision-able Flag
        /// </summary>
        YasuoWall = 1 << 2,

        /// <summary>
        ///     Braum's Shield Collision-able Flag
        /// </summary>
        BraumShield = 1 << 3,

        /// <summary>
        ///     Wall Collision-able Flag
        /// </summary>
        Walls = 1 << 4
    }
}