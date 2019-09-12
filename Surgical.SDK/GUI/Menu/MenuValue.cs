namespace Surgical.SDK.GUI.Menu
{
    using System;

    using Newtonsoft.Json.Linq;

    using Surgical.SDK.Logging;

    public abstract class MenuValue : MenuText
    {
        public bool IsChampSpecific { get; set; }

        private static string ChampNameTag => "xerath";//ObjectManager.Player.CharName.ToLower(); todo

        #region Fields

        private readonly JToken defaultValue;

        private JObject fullToken;

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
            JToken t;

            try
            {
                t = this.Token;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get Token value from item \"{this.Id}\"");
                Log.Error(ex);

                return this.fullToken;
            }

            var @default = JToken.DeepEquals(t, this.defaultValue);

            if (!this.IsChampSpecific)
            {
                return @default ? null : t;
            }

            var full = this.fullToken;

            if (@default)
            {
                if (full != null)
                {
                    full.Remove(ChampNameTag);

                    if (full.Count == 0)
                    {
                        this.fullToken = full = null;
                    }
                }
            }
            else
            {
                (full ??= new JObject())[ChampNameTag] = t;
            }

            return full;
        }

        protected internal sealed override void SetToken(JToken token)
        {
            if (this.IsChampSpecific)
            {
                this.fullToken = (JObject)token;

                token = token?[ChampNameTag];
            }

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
                Log.Warn(ex);

                this.Token = this.defaultValue;
            }
        }

        #endregion
    }
}