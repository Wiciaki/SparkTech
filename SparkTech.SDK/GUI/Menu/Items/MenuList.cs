namespace SparkTech.SDK.GUI.Menu.Items
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Game;

    public class MenuList : MenuValue, IMenuValue<int>, IMenuValue<string>
    {
        private const string ArrowText = ">";

        private int index;

        private Size size;

        private readonly List<string> options;

        private List<Size> sizes;

        public IReadOnlyList<string> GetOptions()
        {
            return this.options.AsReadOnly();
        }

        #region Constructors and Destructors

        public MenuList(string id, List<string> options, int defaultIndex = 0) : base(id, defaultIndex)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Count < 2)
            {
                throw new ArgumentException("Not enough options to choose from", nameof(options));
            }

            if (defaultIndex < 0 || defaultIndex >= options.Count)
            {
                throw new ArgumentException("Index out of bounds", nameof(defaultIndex));
            }

            this.options = options;
        }

        #endregion

        protected internal override void SetTranslations(JObject o)
        {
            var token = o["options"];

            if (token != null)
            {
                var iter = token.Value<JArray>().Select(t => t.Value<string>());

                this.options.Clear();
                this.options.AddRange(iter);

                this.RecalculateItems();
            }

            base.SetTranslations(o);
        }

        protected override Size GetSize()
        {
            var width = Math.Max(28, Theme.MeasureText(ArrowText).Width);

            var s = base.GetSize();
            s.Width += width;

            this.size = new Size(width, s.Height);

            this.RecalculateItems();

            return s;
        }

        private void RecalculateItems()
        {
            this.sizes = this.options.ConvertAll(Theme.MeasureText);
            var width = this.sizes.Max(iS => iS.Width);
            this.sizes = this.sizes.ConvertAll(iS => new Size(width, iS.Height));
        }

        private bool selecting;

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            if (!Menu.IsLeftClick(args.Message))
            {
                return;
            }

            if (Menu.IsCursorInside(point, this.Size))
            {
                this.selecting ^= true;
                return;
            }

            if (!this.selecting)
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

            if (!this.selecting)
            {
                return;
            }

            point.X += this.size.Width + Theme.ItemGroupDistance;

            for (var i = 0; i < this.options.Count; i++)
            {
                var s = this.sizes[i];

                Theme.DrawTextBox(point, s, this.options[i], this.Value == i ? SharpDX.Color.Green : Theme.BackgroundColor);

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
    }
}