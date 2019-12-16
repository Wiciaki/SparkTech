﻿namespace Surgical.SDK.GUI.Menu
{
    using SharpDX;

    using Surgical.SDK.EventData;

    public class MenuKeyBool : MenuKey, IMenuValue<bool>
    {
        public MenuKeyBool(string id, Keys defaultValue) : base(id, defaultValue)
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

        protected override void KeyWndProc(WndProcEventArgs args)
        {
            base.KeyWndProc(args);

            if (args.Keys == base.Value)
            {
                this.Value = args.Message == WindowsMessages.KEYDOWN || args.Message == WindowsMessages.KEYUP && (!this.toggle || (this.release ^= true));
            }
        }
    }
}