//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 29/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  TargetSelector.cs is a part of SparkTech
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

namespace Surgical.SDK.TargetSelection
{
    using System.Collections.Generic;

    using Surgical.SDK.Entities;
    using Surgical.SDK.Modules;

    public static class TargetSelector
    {
        #region Static Fields

        private static readonly IModulePicker<ITargetSelector> Picker; //= SdkSetup.CreatePicker<ITargetSelector, DefaultTargetSelector>();

        #endregion

        #region Public Methods and Operators

        public static void Add<T>(string moduleName) where T : ITargetSelector, new()
        {
            Picker.Add<T>(moduleName);
        }

        public static IHero SelectTarget(this IEnumerable<IHero> heroes)
        {
            return Picker.Current.SelectTarget(heroes);
        }

        #endregion
    }
}