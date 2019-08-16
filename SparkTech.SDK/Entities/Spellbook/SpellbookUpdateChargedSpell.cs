namespace SparkTech.SDK.Entities
{
    using SharpDX;

    using SparkTech.SDK.Misc;

    public class SpellbookUpdateChargedSpell : BlockableEventArgs
    {
        public readonly SpellSlot Slot;

        public readonly Vector3 Position;

        public readonly bool ReleaseCast;

        public SpellbookUpdateChargedSpell(SpellSlot slot, Vector3 position, bool releaseCast)
        {
            this.Slot = slot;

            this.Position = position;

            this.ReleaseCast = releaseCast;
        }
    }
}