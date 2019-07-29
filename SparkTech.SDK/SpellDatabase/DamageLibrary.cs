//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 06/08/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  DamageLibrary.cs is a part of SparkTech
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

namespace SparkTech.SDK.SpellDatabase
{
    using SparkTech.SDK.Enumerations;

    public static class DamageLibrary
    {
        #region Public Methods and Operators

        public static float GetAutoAttackDamage(
            this AIBaseClient attacker,
            AttackableUnit target,
            float? healthSimulated = null)
        {
            throw new NotImplementedException();
            //return Picker.Current.AutoAttackDamage(attacker, target, healthSimulated);
        }

        public static float GetSpellDamage(
            this AIHeroClient attacker,
            AIBaseClient target,
            DamageSlot slot,
            DamageStage stage,
            float? healthSimulated = null)
        {
            throw new NotImplementedException();
            //return Picker.Current.SpellDamage(attacker, target, slot, stage, healthSimulated);
        }

        public static float GetSummonerSpellDamage(
            this AIHeroClient attacker,
            AIBaseClient target,
            SummonerSpell spell,
            DamageStage stage)
        {
            throw new NotImplementedException();
            //return Picker.Current.SummonerSpellDamage(attacker, target, spell, stage);
        }

        public static Version GetVersion()
        {
            //return Picker.Current.GetVersion();
            throw new NotImplementedException();
        }

        #endregion
    }
}