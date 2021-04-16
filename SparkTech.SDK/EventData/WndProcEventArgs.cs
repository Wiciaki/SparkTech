namespace SparkTech.SDK.EventData
{
    using Input;

    public class WndProcEventArgs : BlockableEventArgs
    {
        public uint Msg { get; }

        public uint WParam { get; }

        public int LParam { get; }

        public WindowsMessages Message => (WindowsMessages)this.Msg;

        public Key Key => (Key)this.WParam;

        public WndProcEventArgs(uint msg, uint wParam, int lParam)
        {
            this.Msg = msg;
            this.WParam = wParam;
            this.LParam = lParam;
        }

        public override string ToString()
        {
            return $"WndProc{{Msg={this.Msg},WParam={this.WParam},LParam={this.LParam}}}";
        }
    }
}