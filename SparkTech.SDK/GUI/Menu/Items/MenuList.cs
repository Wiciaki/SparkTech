﻿namespace SparkTech.SDK.GUI.Menu.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Logging;

    public class MenuList : MenuValue, IMenuValue<int>, IMenuValue<string>, IMenuValue<List<string>>, IExpandable
    {
        private const string ArrowText = ">";

        private int index;

        private Size2 size;

        private readonly List<string> options;

        public bool IsExpanded { get; set; }

        private List<Size2> sizes;

        #region Constructors and Destructors

        public MenuList(string id, int defaultIndex = 0) : base(id, defaultIndex)
        {
            this.options = new List<string>();
        }

        #endregion

        protected internal override void SetTranslations(JObject o)
        {
            var token = o["options"];

            if (token != null)
            {
                this.SetOptions(token.Value<JArray>().Select(t => t.Value<string>()).ToArray());
            }

            base.SetTranslations(o);
        }

        private void SetOptions(IReadOnlyCollection<string> items)
        {
            if (items.Count == this.options.Count)
            {
                this.options.Clear();
                this.options.AddRange(items);

                this.RecalculateItems();
            }
            else
            {
                Log.Warn("Provided options count doesn't match!");
            }
        }

        protected override Size2 GetSize()
        {
            var width = Math.Max(28, Theme.MeasureText(ArrowText).Width);

            var s = base.GetSize();
            s.Width += width;

            this.size = new Size2(width, s.Height);

            this.RecalculateItems();

            return s;
        }

        private void RecalculateItems()
        {
            this.sizes = this.options.ConvertAll(Theme.MeasureText);
            var width = this.sizes.Max(iS => iS.Width);
            this.sizes = this.sizes.ConvertAll(iS => new Size2(width, iS.Height));
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            if (!this.IsExpanded || !Menu.IsLeftClick(args.Message))
            {
                return;
            }

            point.X += width + Theme.ItemGroupDistance;

            for (var i = 0; i < this.options.Count; i++)
            {
                var s = this.sizes[i];

                if (Menu.IsCursorInside(point, s))
                {
                    this.Value = i;

                    break;
                }

                point.Y += s.Height;
            }
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawTextBox(point, this.size, ArrowText);

            if (!this.IsExpanded)
            {
                return;
            }

            point.X += this.size.Width + Theme.ItemGroupDistance;

            for (var i = 0; i < this.options.Count; i++)
            {
                var s = this.sizes[i];

                Theme.DrawTextBox(point, s, this.options[i], this.Value == i ? Color.LightGreen : Theme.BackgroundColor);

                point.Y += s.Height;
            }
        }

        #region Public Properties

        public int Value
        {
            get => this.index;
            set
            {
                if (value < 0 || value >= this.options.Count)
                {
                    throw new ArgumentException("The specified index was out of range");
                }

                if (this.index != value && this.UpdateValue(value) && this.UpdateValue(this.options[value]))
                {
                    this.index = value;
                }
            }
        }

        string IMenuValue<string>.Value
        {
            get => this.options[this.index];
            set
            {
                var i = this.options.IndexOf(value);

                if (i == -1)
                {
                    throw new InvalidOperationException($"The item \"{value}\" was not included in the list! (Translations active?)");
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

        List<string> IMenuValue<List<string>>.Value
        {
            get => this.options.ToList();
            set
            {
                if (this.options.SequenceEqual(value) && this.UpdateValue(value))
                {
                    this.SetOptions(value);
                }
            }
        }
    }
}