namespace SparkTech.SDK.GUI.Menu
{
    using SharpDX;

    using SparkTech.SDK.EventData;

    public class MenuKeyBool : MenuKey, IMenuValue<bool>
    {
        public MenuKeyBool(string id, Key defaultValue, bool toggle = false) : base(id, defaultValue)
        {
            this.toggle = toggle;
        }

        private readonly bool toggle;

        private bool value, release;

        protected override Color ButtonColor => this.Value ? Color.OrangeRed : Color.DarkRed;

        public new bool Value
        {
            get => this.value;
            set => this.value ^= this.value != value && this.UpdateValue(value, true);
        }

        protected override void KeyWndProc(WndProcEventArgs args)
        {
            base.KeyWndProc(args);

            if (args.Key != base.Value)
            {
                return;
            }

            var m = args.Message;

            this.Value = this.toggle ? m == WindowsMessages.KEYUP && (this.release ^= true) : m == WindowsMessages.KEYDOWN || m == WindowsMessages.CHAR;
        }
    }
}