//  -------------------------------------------------------------------
//
//  Last updated: 28/09/2017
//  Created: 28/09/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  Buff.cs is a part of SparkTech
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

namespace SparkTech.SDK.Entities.Buffs
{
    using System;

    public static class Extensions
    {
        #region Public Methods and Operators

        public static bool IsMovementImpairing(this IBuff buff)
        {
            return buff.Type().IsMovementImpairing();
        }

        public static bool IsMovementImpairing(this BuffType buffType)
        {
            switch (buffType)
            {
                case BuffType.Flee:
                case BuffType.Charm:
                case BuffType.Slow:
                case BuffType.Snare:
                case BuffType.Stun:
                case BuffType.Taunt:
                    return true;
                default:
                    return false;
            }
        }

        public static bool PreventsCasting(this IBuff buff)
        {
            return buff.Type().PreventsCasting();
        }

        public static bool PreventsCasting(this BuffType buffType) // todo review?
        {
            switch (buffType)
            {
                case BuffType.Silence:
                case BuffType.Charm:
                case BuffType.Taunt:
                case BuffType.Knockup:
                case BuffType.Flee:
                case BuffType.Suppression:
                case BuffType.Stun:
                case BuffType.Polymorph:
                case BuffType.Disarm:
                case BuffType.Knockback:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsValid(this IBuff buff)
        {
            return buff != null && Game.ClockTime >= buff.StartTime() && Game.ClockTime < buff.EndTime();
        }

        public static float TimeLeft(this IBuff buff)
        {
            return Math.Max(0, buff.ExpireTime - Game.ClockTime);
        }

        #endregion
    }
}