namespace SparkTech.Database
{
    using System;

    public class SpellLink
    {
        public virtual string ChampionName { get; }

        public readonly SpellSlot Slot;

        protected SpellLink(SpellSlot slot)
        {
            switch (slot)
            {
                case SpellSlot.Q:
                case SpellSlot.W:
                case SpellSlot.E:
                case SpellSlot.R:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot));
            }

            this.Slot = slot;
        }

        public SpellLink(string championName, SpellSlot slot) : this(slot)
        {
            this.ChampionName = championName;
        }
    }
}