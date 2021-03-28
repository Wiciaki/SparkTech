namespace SparkTech.Ensoul
{
    using System;

    using EnsoulSharp;

    using SDK.API;
    using SDK.EventData;

    internal class UserInputAPI : IUserInputAPI
    {
        public Action<WndProcEventArgs> WndProcess { get; set; }

        public UserInputAPI()
        {
            Game.OnWndProc += args => this.WndProcess(new WndProcEventArgs(args.Msg, args.WParam, args.LParam));
        }
    }
}
