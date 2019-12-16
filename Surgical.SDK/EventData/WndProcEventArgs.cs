namespace Surgical.SDK.EventData
{
    public class WndProcEventArgs : BlockableEventArgs
    {
        public WindowsMessages Message { get; }

        public Keys Keys { get; }

        public WndProcEventArgs(WindowsMessages message, Keys keys)
        {
            this.Message = message;

            this.Keys = keys;
        }

        public override string ToString()
        {
            return $"WndProc{{Message={this.Message},Keys={this.Keys}}}";
        }
    }
}