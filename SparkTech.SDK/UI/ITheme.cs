//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 01/08/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  ITheme.cs is a part of SparkTech
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

namespace SparkTech.SDK.UI
{
    using System.Drawing;

    using SparkTech.SDK.Modules;

    public interface ITheme : IModule
    {
        #region Public Methods and Operators

        Size ItemDistance { get; }

        Size MeasureSize(string text);

        void Draw(DrawData data);

        Color BackgroundColor { get; }

        Color FontColor { get; }

        #endregion
    }
}