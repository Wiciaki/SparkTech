namespace SparkTech.SDK.GUI.Menu
{
    using System;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Logging;

    public abstract class MenuValue : MenuText
    {
        public bool IsChampSpecific { get; set; } // todo

        #region Fields

        private readonly JToken defaultValue;

        #endregion

        #region Constructors and Destructors

        protected MenuValue(string id, JToken defaultValue) : base(id)
        {
            this.defaultValue = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));
        }

        #endregion

        #region Properties

        protected abstract JToken Token { get; set; }

        #endregion

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
                Log.Error($"Failed to get Token value from item \"{this.Id}\"");
                ex.Log();
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
                Log.Warn($"Recovering from MenuValue \"{this.Id}\" type change...");
                ex.Log();

                this.Token = this.defaultValue;
            }
        }

        #endregion
    }
}