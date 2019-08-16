namespace SparkTech.SDK.Entities
{
    using SharpDX;

    using SparkTech.SDK.Misc;

    public class SpellbookCastSpellEventArgs : BlockableEventArgs
    {
        public readonly Vector3 StartPosition;

        public readonly Vector3 EndPosition;

        public readonly IGameObject Target;

        public readonly SpellSlot Slot;

        public SpellbookCastSpellEventArgs(Vector3 startPos, Vector3 endPos, IGameObject target, SpellSlot spellSlot)
        {
            this.StartPosition = startPos;
            this.EndPosition = endPos;
            this.Target = target;
            this.Slot = spellSlot;
        }
    }
}