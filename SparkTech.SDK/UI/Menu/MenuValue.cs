//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 02/08/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  MenuValue.cs is a part of SparkTech
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

namespace SparkTech.SDK.UI.Menu
{
    using System;
    using System.ComponentModel;

    using Newtonsoft.Json.Linq;

    public abstract class MenuValue : NamedComponent, INotifyPropertyChanged
    {
        #region Fields

        private readonly JToken defaultValue;

        #endregion

        #region Constructors and Destructors

        protected MenuValue(string id, JToken defaultValue) : base(id)
        {
            this.defaultValue = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        protected abstract JToken Token { get; set; }

        #endregion

        public T GetValue<T>() => this.MenuValueCast<T>().Value;

        public void SetValue<T>(T value) => this.MenuValueCast<T>().Value = value;

        public IMenuValue<T> MenuValueCast<T>()
        {
            if (this is IMenuValue<T> v)
            {
                return v;
            }

            throw new InvalidOperationException($"\"{this.Id}\" doesn't implement IMenuValue<{typeof(T).Name}>!");
        }

        #region Methods

        protected internal sealed override JToken GetToken()
        {
            try
            {
                var t = this.Token;

                if (!JToken.DeepEquals(t, this.defaultValue))
                {
                    return t;
                }
            }
            catch (Exception ex)
            {
                ex.LogException($"Failed to get Token value from item \"{this.Id}\"");
            }

            return null;
        }

        protected internal sealed override void SetToken(JToken token)
        {
            if (token == null)
            {
                if (this.defaultValue != null)
                {
                    this.Token = this.defaultValue;
                }

                return;
            }

            try
            {
                this.Token = token;
            }
            catch (Exception ex)
            {
                ex.LogException($"Recovering from MenuValue \"{this.Id}\" type change...");

                this.Token = this.defaultValue;
            }
        }

        private bool shouldSave;

        protected internal override bool ShouldSave()
        {
            var b = this.shouldSave;

            this.shouldSave = false;

            return b;
        }

        protected void OnPropertyChanged(string memberName, bool requiresSaving = true)
        {
            if (requiresSaving)
            {
                this.shouldSave = true;
            }

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        #endregion
    }
}