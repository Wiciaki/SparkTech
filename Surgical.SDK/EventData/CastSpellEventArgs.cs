namespace Surgical.SDK.EventData
{
    using SharpDX;

    using Surgical.SDK.Entities;

    public class CastSpellEventArgs : BlockableEventArgs, IEventArgsSource<ISpellbook>, IEventArgsTarget<IGameObject>
    {
        public ISpellbook Source { get; }

        public Vector3 StartPosition { get; }

        public Vector3 EndPosition { get; }

        public IGameObject? Target { get; }

        public SpellSlot Slot { get; }

        public CastSpellEventArgs(ISpellbook source, Vector3 startPos, Vector3 endPos, IGameObject? target, SpellSlot slot)
        {
            this.Source = source;
            this.StartPosition = startPos;
            this.EndPosition = endPos;
            this.Target = target;
            this.Slot = slot;
        }
    }
}