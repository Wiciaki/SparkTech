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

namespace Surgical.SDK.Orbwalking
{
    using System;

    using Surgical.SDK.Entities;
    using Surgical.SDK.Modules;

    public static class Orbwalker
    {
        private static readonly IModulePicker<IOrbwalker> Picker;

        static Orbwalker()
        {
            //Picker = SdkSetup.CreatePicker<IOrbwalker, DefaultOrbwalker>();

            Picker.ModuleSelected += () =>
            {
                Picker.Current.AfterAttack = AfterAttack;
                Picker.Current.BeforeAttack = BeforeAttack;
            };
        }

        public static void Add<TOrbwalker>(string moduleName) where TOrbwalker : IOrbwalker, new()
        {
            Picker.Add<TOrbwalker>(moduleName);
        }

        public static event Action<BeforeAttackEventArgs> BeforeAttack;

        public static event Action<IAttackable> AfterAttack;
    }
}