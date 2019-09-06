namespace SparkTech.SDK.Entities.EventArgs
{
    using SharpDX;

    public class CastSpellEventArgs : BlockableEventArgs, ISourcedEventArgs<ISpellbook>
    {
        public ISpellbook Source { get; }

        public readonly Vector3 StartPosition, EndPosition;

        public readonly IGameObject Target;

        public readonly SpellSlot Slot;

        public CastSpellEventArgs(ISpellbook source, Vector3 startPos, Vector3 endPos, IGameObject target, SpellSlot slot)
        {
            this.Source = source;

            this.StartPosition = startPos;

            this.EndPosition = endPos;

            this.Target = target;

            this.Slot = slot;
        }
    }
}