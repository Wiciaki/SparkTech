namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class EndEventArgs : EventArgs
    {
        public GameObjectTeam WinningTeam { get; }

        public EndEventArgs(GameObjectTeam winningTeam)
        {
            this.WinningTeam = winningTeam;
        }
    }
}