//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  MenuSlider.cs is a part of SparkTech
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
    using System.Drawing;

    using SparkTech.Rendering;
    using SparkTech.Utils;

    using Newtonsoft.Json.Linq;

    using SharpDX.Mathematics.Interop;

    public class MenuSlider : MenuValue, IMenuValue<int>
    {
        #region Fields

        private int value;

        #endregion

        #region Constructors and Destructors

        public MenuSlider(string id, int defaultValue, int min = 0, int max = 100) : base(id, defaultValue)
        {
            if (min > max)
            {
                throw new InvalidOperationException($"The specified {nameof(min)} was greater than {nameof(max)}");
            }

            if (defaultValue < min || defaultValue > max)
            {
                throw new InvalidOperationException($"The specified {nameof(defaultValue)} was out of range");
            }

            this.Min = min;

            this.Max = max;

            this.AddVariable('0', () => $"({this.Value})");
        }

        #endregion

        protected override string MutateDisplayName(string displayName)
        {
            return displayName + " {0}";
        }

        protected internal override void UpdateSize()
        {
            if (!this.dragging)
            {
                base.UpdateSize();
            }
        }

        #region Public Properties

        public int Max { get; }

        public int Min { get; }

        public int Value
        {
            get => this.value;
            set
            {
                if (value == this.value)
                {
                    return;
                }

                if (value > this.Max || value < this.Min)
                {
                    throw new InvalidOperationException("The specified value was out of range");
                }

                this.value = value;

                this.OnPropertyChanged("SelectedValue");

                this.UpdateVariables();
            }
        }
        

        #endregion

        #region Properties

        private const int SliderHeight = 20;

        protected override Size GetSize()
        {
            var size = base.GetSize();

            size.Height += SliderHeight;

            return size;
        }

        protected internal override void OnEndScene(Point point, Size size)
        {
            size.Height -= SliderHeight;

            base.OnEndScene(point, size);

            var range = this.Max - this.Min;

            if (this.dragging)
            {
                var x = (int)Game.ScreenCursorPos.X;

                var diff = x - point.X;

                this.Value = diff <= 0 ? this.Min : x >= point.X + size.Width ? this.Max : this.Min + (int)((float)diff * range / size.Width);
            }

            point.Y += size.Height;
            size.Height = SliderHeight;

            Theme.Draw(new DrawData(point, size));

            point.Y += SliderHeight / 2;

            var offset = (int)(size.Width / (range / (float)(this.Value - this.Min)));
            var color = Theme.FontColor.ToSharpDXColor();

            if (this.dragging)
            {
                color.A = byte.MaxValue;
            }

            Line.Render(color, SliderHeight, true, new RawVector2(point.X, point.Y), new RawVector2(point.X + offset, point.Y));
        }

        private bool dragging;

        protected internal override void OnWndProc(Point point, Size size, GameWndProcEventArgs args)
        {
            point.Y += size.Height - 20;

            if (!this.dragging && !Mouse.IsInside(point, new Size(size.Width, 20)))
            {
                return;
            }

            if (args.Message.IsLeftClick())
            {
                this.dragging = true;
            }
            else if (args.Message == WindowMessage.LBUTTONUP)
            {
                this.dragging = false;

                this.UpdateSize();
            }
        }

        protected override JToken Token
        {
            get => this.value;
            set
            {
                this.value = value.Value<int>();

                this.UpdateVariables();
            }
        }

        #endregion
    }
}