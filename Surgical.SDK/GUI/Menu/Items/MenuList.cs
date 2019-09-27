﻿namespace Surgical.SDK.GUI.Menu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    using Surgical.SDK.EventData;

    public class MenuList : MenuValue, IExpandable, IMenuValue<int>, IMenuValue<string>
    {
        public bool IsExpanded { get; set; }

        private int index;

        private Size2 size;

        private readonly List<string> options;

        private List<Size2> sizes;

        private const string ArrowText = ">";

        #region Constructors and Destructors

        public MenuList(string id, int defaultIndex = 0) : base(id, defaultIndex)
        {
            this.options = new List<string>();
        }

        #endregion

        protected internal override void SetTranslations(Translations t)
        {
            var token = t.GetToken("options");

            if (token != null)
            {
                this.SetOptions(token.Value<JArray>().Select(o => o.Value<string>()).ToArray());
            }

            base.SetTranslations(t);
        }

        private void SetOptions(IList<string> items)
        {
            this.options.Clear();
            this.options.AddRange(items);

            this.RecalculateSizes();
        }

        protected override Size2 GetSize()
        {
            var s = AddButton(base.GetSize(), out this.size, Menu.ArrowText);

            this.RecalculateSizes();

            return s;
        }

        private void RecalculateSizes()
        {
            if (this.options.Count == 0)
            {
                this.sizes = new List<Size2>(0);
                return;
            }

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

            point.X += width;

            if (Menu.ArrowsEnabled)
            {
                point.X += Menu.ArrowWidth;
            }

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

            Theme.DrawTextBox(point, this.size, ArrowText, true, this.BackgroundColor);

            if (!this.IsExpanded)
            {
                return;
            }

            point.X += this.size.Width;

            if (Menu.ArrowsEnabled)
            {
                Menu.AddArrow(point);
                point.X += Menu.ArrowWidth;
            }

            for (var i = 0; i < this.options.Count; i++)
            {
                var s = this.sizes[i];

                Theme.DrawTextBox(point, s, this.options[i], true, this.Value == i ? Color.Green : Theme.BackgroundColor);
                Theme.DrawBorders(point, s);

                point.Y += s.Height;
            }
        }

        #region Public Properties

        public List<string> Options
        {
            get => this.options.ToList();
            set
            {
                if (this.options.SequenceEqual(value))
                {
                    return;
                }

                this.SetOptions(value);

                this.Value = Math.Min(this.Value, value.Count - 1);
            }
        }

        public int Value
        {
            get => this.index;
            set
            {
                if (value < 0 || value >= this.options.Count)
                {
                    throw new ArgumentException("The specified index was out of range");
                }

                if (this.index != value && this.UpdateValue(value))
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
                    throw new ArgumentException($"The item \"{value}\" was not included in the list! (Translations active?)", nameof(value));
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
    }
}