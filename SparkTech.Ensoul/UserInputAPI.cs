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
            Game.OnWndProc += args =>
            {
                var arg = new WndProcEventArgs(args.Msg, args.WParam, args.LParam);
                this.WndProcess(arg);
                if (arg.IsBlocked) args.Process = false;
            };
        }
    }
}
