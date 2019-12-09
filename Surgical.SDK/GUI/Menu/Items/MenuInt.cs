namespace Surgical.SDK.GUI.Menu
{
    using System;

    using Newtonsoft.Json.Linq;

    public class MenuInt : MenuFloat, IMenuValue<int>
    {
        #region Constructors and Destructors

        public MenuInt(string id, int from, int to, int defaultValue) : base(id, from, to, (JToken)defaultValue)
        {
            this.CheckBounds(defaultValue);
        }

        #endregion

        public new int From => (int)base.From;

        public new int To => (int)base.To;

        #region Public Properties

        public new int Value
        {
            get => (int)base.Value;
            set => base.Value = value;
        }

        protected override bool InvokeNotifier(float num)
        {
            return this.UpdateValue((int)num);
        }

        protected override string GetMaxNumStr()
        {
            return base.GetMaxNumStr().Replace(".00", string.Empty, StringComparison.InvariantCulture);
        }

        protected override string GetPrintableStr(float num)
        {
            return "[" + (int)num + "]";
        }

        #endregion

        #region Properties

        protected override JToken Token
        {
            get => (int)base.Token.Value<float>();
            set => base.Token = value.Value<int>();
        }

        #endregion
    }
}