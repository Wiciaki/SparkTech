namespace SparkTech.SDK.Game
{
    using SparkTech.SDK.Misc;

    public class WndProcEventArgs : BlockableEventArgs
    {
        public readonly WindowsMessages Message;

        public readonly WindowsMessagesWParam WParam;

        public readonly int LParam;

        public WndProcEventArgs(WindowsMessages message, WindowsMessagesWParam wparam, int lparam)
        {
            this.Message = message;

            this.WParam = wparam;

            this.LParam = lparam;
        }
    }
}