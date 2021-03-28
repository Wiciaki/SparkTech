namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class StopCastEventArgs : EventArgs, IEventArgsSource<ISpellbook>
    {
        public ISpellbook Source { get; }

        public int MissileId { get; }

        public IMissile Missile => ObjectManager.GetById<IMissile>(this.MissileId);

        public bool KeepAnimationPlaying { get; }

        public bool HasBeenCast { get; }

        public bool SpellStopCancelled { get; }

        public bool DestroyMissile { get; }

        public int SpellCastId { get; }

        public StopCastEventArgs(ISpellbook source, int missileId, bool keepAnimationPlaying, bool hasBeenCast, bool spellStopCancelled, bool destroyMissile, int spellCastId)
        {
            this.Source = source;
            this.MissileId = missileId;
            this.KeepAnimationPlaying = keepAnimationPlaying;
            this.HasBeenCast = hasBeenCast;
            this.SpellStopCancelled = spellStopCancelled;
            this.DestroyMissile = destroyMissile;
            this.SpellCastId = spellCastId;
        }
    }
}