namespace SparkTech.SDK.EventData
{
    using SharpDX;

    using SparkTech.SDK.Entities;

    public class CastSpellEventArgs : BlockableEventArgs, IEventArgsSource<ISpellbook>, IEventArgsTarget<IGameObject>
    {
        public ISpellbook Source { get; }

        public Vector3 StartPosition { get; }

        public Vector3 EndPosition { get; }

        public int TargetId { get; }

        public IGameObject Target => ObjectManager.GetById(this.TargetId); // probably IAttackable

        public SpellSlot Slot { get; }

        public CastSpellEventArgs(ISpellbook source, Vector3 startPos, Vector3 endPos, int targetId, SpellSlot slot)
        {
            this.Source = source;
            this.StartPosition = startPos;
            this.EndPosition = endPos;
            this.TargetId = targetId;
            this.Slot = slot;
        }
    }
}