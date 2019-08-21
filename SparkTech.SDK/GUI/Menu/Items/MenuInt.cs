namespace SparkTech.SDK.GUI.Menu
{
    using Newtonsoft.Json.Linq;

    public class MenuInt : MenuFloat, IMenuValue<int>
    {
        #region Constructors and Destructors

        public MenuInt(string id, int from, int to, int defaultValue) : base(id, from, to, (JToken)defaultValue)
        {
            this.From = from;

            this.To = to;
        }

        #endregion

        #region Public Properties

        public new readonly int From;

        public new readonly int To;

        public new int Value
        {
            get => (int)base.Value;
            set => this.SetFloat(value);
        }

        protected override void SetFloat(float num)
        {
            if (this.UpdateValue((int)num))
            {
                base.SetFloat(num);
            }
        }

        protected override string GetMaxNumStr()
        {
            return base.GetMaxNumStr().Replace(".00", "");
        }

        protected override string GetPrintableStr(float num)
        {
            return ((int)num).ToString();
        }

        #endregion

        #region Properties

        protected override JToken Token
        {
            get => this.Value;
            set => this.Value = value.Value<int>();
        }

        #endregion
    }
}