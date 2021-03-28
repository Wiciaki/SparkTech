namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class _EndEventArgs : EventArgs
    {
        public GameObjectTeam WinningTeam { get; }

        public _EndEventArgs(GameObjectTeam winningTeam)
        {
            this.WinningTeam = winningTeam;
        }
    }
}