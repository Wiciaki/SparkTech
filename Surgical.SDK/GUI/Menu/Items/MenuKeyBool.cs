namespace Surgical.SDK.GUI.Menu
{
    using SharpDX;

    using Surgical.SDK.EventData;

    public class MenuKeyBool : MenuKey, IMenuValue<bool>
    {
        public MenuKeyBool(string id, Key defaultValue) : base(id, defaultValue)
        {

        }

        private bool value, toggle, release;

        public new bool Value
        {
            get => this.value;
            set => this.value ^= this.value != value && this.UpdateValue(value, true);
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

        protected override void KeyWndProc(WndProcEventArgs args)
        {
            base.KeyWndProc(args);

            if (args.Key != base.Value)
            {
                return;
            }

            if (this.Toggle)
            {
                this.Value = args.Message == WindowsMessages.KEYUP && (this.release ^= true);
            }
            else
            {
                this.Value = args.Message == WindowsMessages.KEYDOWN || args.Message == WindowsMessages.CHAR;
            }
        }
    }
}