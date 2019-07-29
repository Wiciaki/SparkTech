//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  MenuButton.cs is a part of SparkTech
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
    using System;
    using System.Drawing;

    using SparkTech.SDK.Util;

    public class MenuButton : NamedComponent
    {
        public event Action OnPress;

        public MenuButton(string id, Action onPress = null) : base(id)
        {
            if (onPress != null)
            {
                this.OnPress += onPress;
            }
        }

        private bool pressing;

        private Size buttonSize;

        protected override Size GetSize()
        {
            var size = base.GetSize();

            this.buttonSize = new Size(size.Height, size.Height);

            size.Width += size.Height;

            return size;
        }

        protected internal override void OnEndScene(Point point, Size size)
        {
            size.Width -= size.Height;

            base.OnEndScene(point, size);

            point.X += size.Width;

            Theme.Draw(new DrawData(point, this.buttonSize) { BackgroundColor = this.pressing ? Color.Gray : Color.DarkGray });
        }

        protected internal override void OnWndProc(Point point, Size size, GameWndProcEventArgs args)
        {
            point.X += size.Width - size.Height;

            if (!Mouse.IsInside(point, this.buttonSize))
            {
                this.pressing = false;
                return;
            }

            if (args.Message.IsLeftClick())
            {
                this.pressing = true;
                return;
            }

            if (!this.pressing || args.Message != WindowMessage.LBUTTONUP)
            {
                return;
            }

            this.pressing = false;

            this.OnPress.SafeInvoke(nameof(this.OnPress));
        }
    }
}