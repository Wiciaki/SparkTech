//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  MenuList.cs is a part of SparkTech
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

namespace SparkTech.UI.Menu.Values
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;

    using SparkTech.Caching;
    using SparkTech.Utils;

    using Newtonsoft.Json.Linq;

    public class MenuList : MenuValue, IMenuValue<int>, IMenuValue<string>
    {
        #region Fields

        private int index;

        #endregion

        private Size itemSize;

        #region Constructors and Destructors

        public MenuList(string id, List<string> options, string defaultValue) : this(id, options, options.IndexOf(defaultValue))
        {

        }

        public MenuList(string id, List<string> options, int defaultIndex = 0) : base(id, defaultIndex)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (defaultIndex < 0 || defaultIndex >= options.Count)
            {
                throw new IndexOutOfRangeException();
            }

            this.List = new ObservableCollection<string>(options);

            this.List.CollectionChanged += delegate
            {
                this.List.Remove(this.List.FirstOrDefault(string.IsNullOrWhiteSpace));

                this.UpdateSize();
            };
        }

        #endregion

        public readonly ObservableCollection<string> List;

        protected internal override bool IsExpanding => true;

        protected override Size GetSize()
        {
            this.itemSize = this.List.Select(Theme.MeasureSize).OrderByDescending(s => s.Width).First();

            var size = base.GetSize();

            this.arrowSize = new Size(size.Height, size.Height);

            size.Width += size.Height;

            return size;
        }

        protected internal override void OnWndProc(Point point, Size size, GameWndProcEventArgs args)
        {
            if (!this.IsHighlighted || !args.Message.IsLeftClick())
            {
                return;
            }

            point.X += size.Width + Theme.ItemDistance.Width;

            for (var i = 0; i < this.List.Count; i++)
            {
                if (Mouse.IsInside(point, this.itemSize))
                {
                    this.Value = i;

                    return;
                }

                point.Y += this.itemSize.Height + Theme.ItemDistance.Height;
            }
        }

        private Size arrowSize;

        private void DrawSelf(Point point, Size size)
        {
            size.Width -= this.arrowSize.Width;

            base.OnEndScene(point, size);

            point.X += size.Width;

            Theme.Draw(new DrawData(point, this.arrowSize) { Text = ">", ForceTextCentered = true });
        }

        protected internal override void OnEndScene(Point point, Size size)
        {
            this.DrawSelf(point, size);

            if (!this.IsHighlighted)
            {
                return;
            }

            point.X += size.Width + Theme.ItemDistance.Width;

            for (var i = 0; i < this.List.Count; i++)
            {
                Theme.Draw(new DrawData(point, this.itemSize) { Text = this.List[i], BackgroundColor = this.index == i ? Color.Green : Color.Red });

                point.Y += this.itemSize.Height + Theme.ItemDistance.Height;
            }
        }

        #region Public Properties

        public int Value
        {
            get => this.index;
            set
            {
                if (value < 0 || value >= this.List.Count)
                {
                    throw new InvalidOperationException("The specified index was out of range");
                }

                if (this.index == value)
                {
                    return;
                }

                this.index = value;

                this.OnPropertyChanged("Index");
                this.OnPropertyChanged("Selection");
            }
        }

        string IMenuValue<string>.Value
        {
            get => this.List[this.index];
            set
            {
                var i = this.List.IndexOf(value);

                if (i == -1)
                {
                    throw new InvalidOperationException($"The item \"{value}\" was not included in the list!");
                }

                this.Value = i;
            }
        }

        #endregion

        #region Properties

        protected override JToken Token
        {
            get => this.index;
            set => this.index = value.Value<int>();
        }

        #endregion

        #region Public Methods and Operators

        public TEnum Enum<TEnum>() where TEnum : struct, IConvertible
        {
            return EnumCache<TEnum>.Parse(this.GetValue<string>());
        }

        #endregion
    }
}