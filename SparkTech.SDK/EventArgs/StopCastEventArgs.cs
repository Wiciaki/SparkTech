namespace SparkTech.SDK.EventArgs
{
    using System;

    using SparkTech.SDK.Entities;

    public class StopCastEventArgs : EventArgs, ISourcedEventArgs<ISpellbook>
    {
        public ISpellbook Source { get; }

        public readonly bool StopAnimation, ExecuteCastFrame, ForceStop, DestroyMissile;

        public readonly int MissileId;

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