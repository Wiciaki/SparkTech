//  -------------------------------------------------------------------
//
//  Last updated: 28/09/2017
//  Created: 17/08/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  Minion.cs is a part of SparkTech
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
    using System.Text.RegularExpressions;

    using SparkTech.Enumerations;

    #endregion

    public static class Minion
    {
        #region Static Fields

        private static readonly Regex LaneMinionRegex = new Regex(
            "(?:SRU|HA)_(?:Chaos|Order)Minion(.*)",
            RegexOptions.Compiled | RegexOptions.Singleline);

        #endregion

        #region Public Methods and Operators

        public static MinionType GetMinionType(this AIMinion minion)
        {
            if (minion == null) return MinionType.Unknown;

            var baseSkinName = minion.CharName;

            var match = LaneMinionRegex.Match(baseSkinName);

            if (match.Success)
            {
                var value = match.Groups[1].Value;

                switch (value)
                {
                    case "Melee": return MinionType.Melee;
                    case "Ranged": return MinionType.Caster;
                    case "Siege": return MinionType.Siege;
                    case "Super": return MinionType.Super;
                    default: throw new ArgumentOutOfRangeException(nameof (value));
                }
            }

            if (baseSkinName.StartsWith("SRU_Plant", StringComparison.CurrentCultureIgnoreCase))
            {
                return MinionType.Plant;
            }

            baseSkinName = baseSkinName.ToLower();

            if (baseSkinName.Contains("ward") || baseSkinName.Contains("trinket"))
            {
                return MinionType.Ward;
            }

            var name = minion.Name;

            if (name.StartsWith("SRU_", StringComparison.OrdinalIgnoreCase))
            {
                return name.EndsWith("Mini") ? MinionType.JungleMini : MinionType.Jungle;
            }

            // other ???
            return MinionType.Summoned; // trundle wall, Jayce gate, heimer turret, malzahar voidling
        }

        public static bool IsJungle(this AIMinion minion)
        {
            return minion.GetMinionType().IsJungle();
        }

        public static bool IsJungle(this MinionType type)
        {
            return type == MinionType.Jungle || type == MinionType.JungleMini;
        }

        public static bool IsJungleBuff(this AIMinion minion)
        {
            switch (minion.CharName)
            {
                case "SRU_Blue":
                case "SRU_Red":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsMinion(this AIMinion minion)
        {
            return minion.GetMinionType().IsMinion();
        }

        public static bool IsMinion(this MinionType type)
        {
            switch (type)
            {
                case MinionType.Melee:
                case MinionType.Caster:
                case MinionType.Siege:
                case MinionType.Super:
                    return true;
                default:
                    return false;
            }
        }

        #endregion
    }
}