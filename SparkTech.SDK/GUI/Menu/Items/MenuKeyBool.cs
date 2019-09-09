namespace SparkTech.SDK.GUI.Menu
{
    using SharpDX;

    using SparkTech.SDK.EventArgs;

    public class MenuKeyBool : MenuKey, IMenuValue<bool>
    {
        public MenuKeyBool(string id, Key defaultValue) : base(id, defaultValue)
        {

        }

        private bool value, toggle, release;

        public new bool Value
        {
            get => this.value;
            set => this.value ^= this.value != value && this.UpdateValue(value);
        }

        public bool Toggle
        {
            get => this.toggle;
            set
            {
                if (this.toggle == value)
                {
                    return;
                }

                this.toggle = value;

                this.release = false;
                this.Value = false;
            }
        }

        protected override Color ButtonColor => this.Value ? Color.OrangeRed : Color.DarkRed;

        protected override void WndProc(WndProcEventArgs args)
        {
            base.WndProc(args);

            if (args.WParam == base.Value)
            {
                this.Value = args.Message == WindowsMessages.KEYDOWN || args.Message == WindowsMessages.KEYUP && (!this.toggle || (this.release ^= true));
            }
        }
    }
}