namespace SparkTech.SDK.EventArgs
{
    using SharpDX;

    using SparkTech.SDK.Entities;

    public class UpdateChargedSpellEventArgs : BlockableEventArgs, ISourcedEventArgs<ISpellbook>
    {
        public ISpellbook Source { get; }

        public readonly SpellSlot Slot;

        public readonly Vector3 Position;

        public readonly bool ReleaseCast;

        public UpdateChargedSpellEventArgs(ISpellbook source, SpellSlot slot, Vector3 position, bool releaseCast)
        {
            this.Source = source;

            this.Slot = slot;

            this.Position = position;

            this.ReleaseCast = releaseCast;
        }
    }
}