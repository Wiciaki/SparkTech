namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class StopCastEventArgs : EventArgs, ISourcedEventArgs<ISpellbook>
    {
        public ISpellbook Source { get; }

        public bool StopAnimation { get; }

        public bool ExecuteCastFrame { get; }

        public bool ForceStop { get; }

        public bool DestroyMissile { get; }

        public int MissileId { get; }

        public StopCastEventArgs(ISpellbook source, bool stopAnimation, bool executeCastFrame, bool forceStop, bool destroyMissile, int missileId)
        {
            this.Source = source;

            this.StopAnimation = stopAnimation;

            this.ExecuteCastFrame = executeCastFrame;

            this.ForceStop = forceStop;

            this.DestroyMissile = destroyMissile;

            this.MissileId = missileId;
        }
    }
}