namespace Surgical.SDK.EventData
{
    public class InputEventArgs : BlockableEventArgs
    {
        public string Input { get; }

        public InputEventArgs(string input)
        {
            this.Input = input;
        }
    }
}