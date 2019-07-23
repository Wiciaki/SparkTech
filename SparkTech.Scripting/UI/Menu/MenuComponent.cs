//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  MenuComponent.cs is a part of SparkTech
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

namespace SparkTech.UI.Menu
{
    using System;
    using System.Drawing;

    using Newtonsoft.Json.Linq;

    public abstract class MenuComponent
    {
        public readonly string Id;

        protected MenuComponent(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"The provided id \"{id}\" was invalid!");
            }

            this.Id = id;

            Theme.Proxy.ThemeSelected += this.UpdateSize;
        }

        internal static Point Mouse;

        public bool IsVisible = true;

        public TMenuComponent Cast<TMenuComponent>() where TMenuComponent : MenuComponent
        {
            return (TMenuComponent)this;
        }

        protected internal virtual bool IsExpanding => false;

        protected internal bool IsHighlighted;

        protected internal abstract void OnEndScene(Point point, Size size);

        protected abstract Size GetSize();

        protected internal Size Size { get; private set; }

        protected internal virtual void UpdateSize()
        {
            this.Size = this.GetSize();
        }

        protected internal virtual void OnWndProc(Point point, Size size, GameWndProcEventArgs args)
        { }

        protected internal virtual void OnDrawOverlay(Point point, Size size)
        { }

        protected internal virtual bool ShouldSave() => false;

        protected internal virtual JToken GetToken() => null;

        protected internal virtual void SetToken(JToken token)
        { }
    }
}