﻿//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 05/08/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  CollisionableObjects.cs is a part of Entropy.SDK
// 
//  Entropy.SDK is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  Entropy.SDK is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  You should have received a copy of the GNU General Public License
//  along with Entropy.SDK. If not, see <http://www.gnu.org/licenses/>.
// 
//  -------------------------------------------------------------------

namespace SparkTech.SDK.MovementPrediction
{
    using System;

    /// <summary>
    ///     Enum CollisionableObjects
    /// </summary>
    [Flags]
    public enum CollisionableObjects
    {
        Minions = 1,
        Heroes = 2,
        YasuoWall = 3,
        Walls = 4
    }
}