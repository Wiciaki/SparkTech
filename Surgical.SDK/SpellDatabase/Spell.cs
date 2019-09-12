namespace Surgical.SDK.SpellDatabase
{
    /*
    using SparkTech.SDK.TickOperations;

    public class Spell : SpellLink
    {
        public readonly AIHeroClient Owner;

        public SpellDataInst SpellDataInst { get; private set; }

        private Spellbook spellbook;

        public Spell(AIHeroClient owner, SpellSlot slot) : base(slot)
        {
            this.Owner = owner;

            GameLoading.OnLoad += delegate
            {
                this.SpellDataInst = owner.Spellbook.GetSpell(slot);

                this.spellbook = owner.Spellbook;
            };
        }

        /// <summary>
        /// Spell's state
        /// </summary>
        public SpellState GetState()
        {
            return this.spellbook?.GetSpellState(this.Slot) ?? SpellState.Unknown;
        }

        /// <summary>
        /// Returns a value indicating whether the current spell instance is ready.
        /// </summary>
        public bool IsReady()
        {
            return this.GetState() == SpellState.Ready;
        }

        public float GetTimeUntilReady()
        {
            switch (this.GetState())
            {
                case SpellState.Ready:
                    return 0f;
                case SpellState.NotAvailable:
                case SpellState.NotLearned:
                case SpellState.Unknown:
                    return -1f;
            }

            var left = 0f;

            void Assign(float time)
            {
                if (time > left)
                {
                    left = time;
                }
            }

            var cooldown = this.SpellDataInst.CooldownExpires - Game.ClockTime;

            if (cooldown > 0f)
            {
                Assign(cooldown);
            }

            foreach (var delta in this.Owner.BuffManager.Buffs.Where(b => b.PreventsCasting()).Select(b => b.ExpireTime - b.StartTime))
            {
                Assign(delta);
            }

            var cost = 0f;//this.DataInst.SpellData.ManaCost[this.DataInst.Level - 1]; todo

            if (cost > 0f)
            { // todo
                cost -= this.Owner.MP;

                if (cost > 0)
                {
                    Assign(cost / this.Owner.CharIntermediate.PrimaryARRegenRateRep);
                }
            }

            return left;
        }

        public float GetRange()
        {
            return 700f;
            //return Database.Get
        }

        /// <summary>
        /// Checks if the Vector3 is in range of the spell
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public bool IsInRange(Vector3 vector3)
        {
            return this.Owner.Distance(vector3) <= this.GetRange();
        }

        /// <summary>
        /// Checks if the Object is in range of the spell
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsInRange(GameObject obj)
        {
            return this.IsInRange(obj.Position);
        }
    }*/
}