//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 29/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  Orbwalker.cs is a part of SparkTech
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

namespace SparkTech.Orbwalking
{
    using System.Collections.Generic;

    using SparkTech.Orbwalking.Default;
    using SparkTech.UI.Menu;

    using BeforeAttack = System.Action<BeforeAttackEventArgs>;
    using AfterAttack = System.Action<AttackableUnit>;

    public static class Orbwalker
    {
        #region Static Fields

        private static readonly List<BeforeAttack> BeforeAttackCallbacks = new List<BeforeAttack>();

        private static readonly List<AfterAttack> AfterAttackCallbacks = new List<AfterAttack>();

        private static readonly IModulePicker<IOrbwalker> Picker;

        static Orbwalker()
        {
            Picker = EntropySetup.CreatePicker<IOrbwalker, DefaultOrbwalker>();

            Picker.ModuleSelected += delegate
            {
                Picker.Current.BeforeAttack += args => BeforeAttackCallbacks.ForEach(c => c(args));

                Picker.Current.AfterAttack += u => AfterAttackCallbacks.ForEach(c => c(u));
            };
        }

        public static void Add<TOrbwalker>(string moduleName) where TOrbwalker : IOrbwalker, new()
        {
            Picker.Add<TOrbwalker>(moduleName);
        }

        #endregion

        public static Mode GetMode() => Picker.Current.GetMode();

        public static float LastAttackStartTime => Picker.Current.LastAttackStartTime;

        public static event BeforeAttack BeforeAttack
        {
            add => BeforeAttackCallbacks.Add(value);
            remove => BeforeAttackCallbacks.Remove(value);
        }

        public static event AfterAttack AfterAttack
        {
            add => AfterAttackCallbacks.Add(value);
            remove => AfterAttackCallbacks.Remove(value);
        }
    }
}