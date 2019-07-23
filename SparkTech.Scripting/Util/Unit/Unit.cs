//  -------------------------------------------------------------------
//
//  Last updated: 28/09/2017
//  Created: 29/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  Unit.cs is a part of SparkTech
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
    using System.Linq;

    using SparkTech.Caching;
    using SparkTech.Enumerations;

    public static class Unit
    {
        #region Public Methods and Operators

        private static readonly GameObjectComparer GameObjectComparer = new GameObjectComparer();

        public static bool Compare(this GameObject left, GameObject right)
        {
            return GameObjectComparer.Equals(left, right);
        }

        public static bool HasItem(this IEnumerable<InventorySlot> inventorySlot, params uint[] itemIds)
        {
            return inventorySlot.Any(i => itemIds.Contains(i.ItemID));
        }

        public static bool HasItem(this AIHeroClient target, params uint[] itemIds)
        {
            return target.InventorySlots.HasItem(itemIds);
        }

        public static bool IsAlly(this GameObject target)
        {
            return target.Team() == ObjectTeam.Ally;
        }

        public static bool IsEnemy(this GameObject target)
        {
            return target.Team() == ObjectTeam.Enemy;
        }

        public static bool IsMe(this GameObject target)
        {
            return target.NetworkID == LocalPlayer.Instance.NetworkID;
        }

        public static bool IsMovementImpaired(this AIBaseClient unit)
        {
            return unit.BuffManager.Buffs.Exists(Buff.IsMovementImpairing);
        }

        public static bool IsValidTarget(this AttackableUnit t, bool mustBeEnemy = true)
        {
            /*

            if (t.Name == "WardCorpse")
            {
                return false;
            }

            */

            return t != null && t.IsVisible && !t.IsDead && t.IsTargetable && !t.IsInvulnerable && (!mustBeEnemy || t.IsEnemy());
        }

        //TODO make this when Orientation is fixed
        public static bool IsFacingUnit(this AIBaseClient source, AIBaseClient target)
        {
            //TODO Server pos
            //return source.Orientation.To2D().Perpendicular().AngleBetween((target.Position - source.Postion).To2D()) < 90;
            return false;
        }

        public static float TotalShieldHealth(this AIBaseClient unit)
        {
            return unit.HP + unit.PhysicalShield + unit.MagicalShield;
        }

        public static float TotalPhysicalShieldHealth(this AIBaseClient unit)
        {
            return unit.HP + unit.PhysicalShield;
        }

        public static float TotalMagicalShieldHealth(this AIBaseClient unit)
        {
            return unit.HP + unit.MagicalShield;
        }

        #endregion
    }
}