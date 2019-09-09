namespace SparkTech.SDK.EventArgs
{
    public class WndProcEventArgs : BlockableEventArgs
    {
        public readonly WindowsMessages Message;

        public readonly Key WParam;

        public WndProcEventArgs(WindowsMessages message, Key wparam)
        {
            this.Message = message;

            this.WParam = wparam;
        }
    }
}