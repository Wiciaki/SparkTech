namespace SparkTech.SDK.Game
{
    public class WndProcEventArgs : BlockableEventArgs
    {
        public readonly uint Msg;

        public readonly uint WParam;

        public readonly int LParam;

        public WndProcEventArgs(uint msg, uint wparam, int lparam)
        {
            this.Msg = msg;

            this.WParam = wparam;

            this.LParam = lparam;
        }
    }
}