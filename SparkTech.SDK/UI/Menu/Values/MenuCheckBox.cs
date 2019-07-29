//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  MenuCheckBox.cs is a part of SparkTech
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

namespace SparkTech.SDK.UI.Menu.Values
{
    using System.Drawing;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Util;

    public class MenuCheckBox : MenuValue, IMenuValue<bool>
    {
        #region Fields

        private bool value;

        #endregion

        #region Constructors and Destructors

        public MenuCheckBox(string id, bool defaultValue)
            : base(id, defaultValue)
        {

        }

        private Size buttonSize;

        protected override Size GetSize()
        {
            var size = base.GetSize();

            this.buttonSize = new Size(size.Height, size.Height);

            size.Width += size.Height;

            return size;
        }

        #endregion

        #region Public Properties

        public bool Value
        {
            get => this.value;
            set
            {
                if (this.value == value)
                {
                    return;
                }

                this.value = value;

                this.OnPropertyChanged("IsActive");
            }
        }

        #endregion

        protected internal override void OnEndScene(Point point, Size size)
        {
            size.Width -= this.buttonSize.Width;

            base.OnEndScene(point, size);

            point.X += size.Width;

            Theme.Draw(new DrawData(point, this.buttonSize) { BackgroundColor = this.value ? Color.Green : Color.Red });
        }

        protected internal override void OnWndProc(Point point, Size size, GameWndProcEventArgs args)
        {
            point.X += size.Width - this.buttonSize.Width;

            this.Value ^= Mouse.IsInside(point, this.buttonSize) && args.Message.IsLeftClick();
        }

        #region Properties

        protected override JToken Token
        {
            get => this.value;
            set => this.value = value.Value<bool>();
        }

        #endregion
    }
}