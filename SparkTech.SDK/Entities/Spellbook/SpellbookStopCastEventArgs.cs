namespace SparkTech.SDK.Entities
{
    public class SpellbookStopCastEventArgs
    {
        public readonly bool StopAnimation;

        public readonly bool ExecuteCastFrame;

        public readonly bool ForceStop;

        public readonly bool DestroyMissile;

        public readonly int MissileId;

        // ??
        //public readonly int Unk;

        public SpellbookStopCastEventArgs(
            bool stopAnimation,
            bool executeCastFrame,
            bool forceStop,
            bool destroyMissile,
            int missileId)
        {
            this.StopAnimation = stopAnimation;
            this.ExecuteCastFrame = executeCastFrame;
            this.ForceStop = forceStop;
            this.DestroyMissile = destroyMissile;
            this.MissileId = missileId;
            // this.Unk = unk;
        }
    }
}