namespace SparkTech.SDK
{
    public class WndProcEventArgs : BlockableEventArgs
    {
        public readonly WindowsMessages Message;

        public readonly WindowsMessagesWParam WParam;

        public WndProcEventArgs(WindowsMessages message, WindowsMessagesWParam wparam)
        {
            this.Message = message;

            this.WParam = wparam;
        }
    }
}