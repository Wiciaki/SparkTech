//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  NamedComponent.cs is a part of SparkTech
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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text.RegularExpressions;

    using SparkTech.Utils;

    using Newtonsoft.Json.Linq;

    public abstract class NamedComponent : MenuComponent
    {
        protected NamedComponent(string id) : base(id)
        {

        }

        private const string DefaultDisplayName = "> Display name missing <";

        public string DisplayName { get; private set; } = DefaultDisplayName;

        public string TooltipText { get; private set; }

        private string baseDisplayName;

        private Dictionary<char, string> savedVariables = new Dictionary<char, string>();

        protected internal virtual void UpdateTranslations(JToken token)
        {
            this.ValidateControl(true);

            this.TooltipText = token?["tooltip"].Value<string>();

            this.DisplayName = token?["name"].Value<string>() ?? DefaultDisplayName;
        }

        protected internal virtual JObject GetRequiredTranslations()
        {
            return new JObject { { "name", string.Empty }, { "tooltip", string.Empty } };
        }

        public NamedComponent SetDisplayName(string displayName)
        {
            this.ValidateControl(false);

            this.UpdateDisplayName(displayName);

            return this;
        }

        public NamedComponent SetTooltipText(string tooltipText)
        {
            this.ValidateControl(false);

            this.TooltipText = tooltipText;

            this.UpdateSize();

            return this;
        }

        public NamedComponent AddVariable(char c, Func<string> variableObtainer)
        {
            this.variables.Add(c, variableObtainer);

            this.DisplayName = this.DisplayName.Replace($"{{{c}}}", variableObtainer());

            this.UpdateSize();

            return this;
        }

        public void UpdateVariables()
        {
            if (this.baseDisplayName == null)
            {
                return;
            }

            if (this.variables.Count == 0)
            {
                if (this.DisplayName == this.baseDisplayName)
                {
                    return;
                }

                this.DisplayName = this.baseDisplayName;
            }
            else
            {
                this.DisplayName = this.variables.Aggregate(this.baseDisplayName, (current, pair) => current.Replace($"{{{pair.Key}}}", pair.Value()));
            }

            this.UpdateSize();
        }

        private bool isControlled;

        private void ValidateControl(bool controlled)
        {
            if (controlled)
            {
                this.isControlled = true;
            }
            else if (this.isControlled)
            {
                throw new InvalidOperationException("Attempted DisplayName change on a controlled multilanguage component!");
            }
        }

        private readonly Dictionary<char, Func<string>> variables = new Dictionary<char, Func<string>>();

        private static readonly Regex VariableFinder = new Regex("{[a-zA-Z0-9]}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        protected virtual string MutateDisplayName(string displayName) => displayName;

        private void UpdateDisplayName(string displayName)
        {
            var count = VariableFinder.Match(displayName).Captures.Count;

            if (count != displayName.Count(c => c == '{') || count != displayName.Count(c => c == '}'))
            {
                throw new ArgumentException(
                    // ReSharper disable once LocalizableElement
                    "Invalid displayName or char out of range! "
                    + "Please note that the \"{\" and \"}\" symbols are to by used for variables only.",
                    nameof(displayName));
            }

            this.baseDisplayName = this.MutateDisplayName(displayName);

            this.UpdateVariables();
        }

        // ------

        private Size tooltipTextSize, tooltipSize;

        protected override Size GetSize()
        {
            var size = Theme.MeasureSize(this.DisplayName);

            if (this.TooltipText != null)
            {
                this.tooltipTextSize = Theme.MeasureSize(this.TooltipText);

                var t = Theme.MeasureSize("[?]");

                size.Width += t.Width;

                this.tooltipSize = t;
            }

            return size;
        }

        private bool drawTooltip;

        protected internal override void OnDrawOverlay(Point point, Size size)
        {
            if (!this.drawTooltip)
            {
                return;
            }

            point.X += size.Width + Theme.ItemDistance.Width;

            Theme.Draw(new DrawData(point, this.tooltipTextSize) { Text = this.TooltipText });
        }

        protected internal override void OnEndScene(Point point, Size size)
        {
            size.Width -= this.tooltipSize.Width;

            Theme.Draw(new DrawData(point, size) { Text = this.DisplayName });

            if (this.TooltipText == null)
            {
                return;
            }

            point.X += size.Width;

            Theme.Draw(new DrawData(point, this.tooltipSize) { Text = "[?]", ForceTextCentered = true });

            this.drawTooltip = this.IsHighlighted && Game.ScreenCursorPos.IsInside(point, this.tooltipSize);
        }
    }
}