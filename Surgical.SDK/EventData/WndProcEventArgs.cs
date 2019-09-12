﻿namespace Surgical.SDK.EventData
{
    public class WndProcEventArgs : BlockableEventArgs
    {
        public WindowsMessages Message { get; }

        public Key Key { get; }

        public WndProcEventArgs(WindowsMessages message, Key key)
        {
            this.Message = message;

            this.Key = key;
        }
    }
}