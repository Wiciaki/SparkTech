namespace SparkTech.SDK.EventData
{
    public class _InputEventArgs : BlockableEventArgs
    {
        public string Input { get; }

        public _InputEventArgs(string input)
        {
            this.Input = input;
        }
    }
}