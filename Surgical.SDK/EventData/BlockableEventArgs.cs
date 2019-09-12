namespace Surgical.SDK.EventData
{
    using System;

    public class BlockableEventArgs : EventArgs
    {
        public bool IsBlocked { get; private set; }

        public void Block()
        {
            this.IsBlocked = true;
        }
    }
}