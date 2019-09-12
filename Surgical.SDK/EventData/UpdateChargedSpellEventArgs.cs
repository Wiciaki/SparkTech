namespace Surgical.SDK.EventData
{
    using SharpDX;

    using Surgical.SDK.Entities;

    public class UpdateChargedSpellEventArgs : BlockableEventArgs, ISourcedEventArgs<ISpellbook>
    {
        public ISpellbook Source { get; }

        public SpellSlot Slot { get; }

        public Vector3 Position { get; }

        public bool ReleaseCast { get; }

        public UpdateChargedSpellEventArgs(ISpellbook source, SpellSlot slot, Vector3 position, bool releaseCast)
        {
            this.Source = source;

            this.Slot = slot;

            this.Position = position;

            this.ReleaseCast = releaseCast;
        }
    }
}