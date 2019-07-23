namespace SparkTech.Database.Default
{
    using System;

    using SparkTech.Enumerations;
    using SparkTech.Properties;
    using SparkTech.UI.Menu;

    using Newtonsoft.Json.Linq;

    internal sealed class DefaultDatabase : IDatabase
    {
        ModuleMenu IEntropyModule.Menu => null;

        void IEntropyModule.Release()
        {
            this.Updated = null;

            Menu.LanguageChanged -= this.Load;
        }

        public Version Version { get; private set; }

        private JObject rawData;

        public event Action Updated;

        private bool loaded;

        private void Load()
        {
            var json = JObject.Parse(Resources.Champion);

            this.rawData = (JObject)json["data"];

            this.Version = new Version(json["version"].ToString());

            if (!this.loaded)
            {
                Menu.LanguageChanged += this.Load;
            }
            else
            {
                this.Updated?.Invoke();
            }

            this.loaded = true;
        }

        public DefaultDatabase()
        {
            this.Load();
        }

        private T GetValue<T>(SpellLink spell, string propertyName)
        {
            return this.rawData[spell.ChampionName][(int)spell.Slot][propertyName].Value<T>();
        }

        string IDatabase.GetName(SpellLink spell)
        {
            return this.GetValue<string>(spell, "name");
        }

        string IDatabase.GetKey(SpellLink spell)
        {
            return this.GetValue<string>(spell, "key");
        }

        float[] IDatabase.GetRange(SpellLink spell)
        {
            return this.GetValue<float[]>(spell, "range");
        }

        float[] IDatabase.GetCooldown(SpellLink spell)
        {
            return this.GetValue<float[]>(spell, "cooldown");
        }

        float[] IDatabase.GetCost(SpellLink spell)
        {
            return this.GetValue<float[]>(spell, "cost");
        }

        int IDatabase.GetMaxRank(SpellLink spell)
        {
            return this.GetValue<int>(spell, "maxrank");
        }

        public float SpellDamage(Spell spell, AIBaseClient target, DamageStage stage, float? healthSimulated = null)
        {
            throw new NotImplementedException();
        }

        public float SummonerSpellDamage(AIHeroClient attacker, AIBaseClient target, SummonerSpell spell, DamageStage stage)
        {
            throw new NotImplementedException();
        }

        public float AutoAttackDamage(AIBaseClient attacker, AttackableUnit target, float? healthSimulated = null)
        {
            return attacker.CharIntermediate.BaseAttackDamage + attacker.CharIntermediate.FlatPhysicalDamageMod;
        }
    }
}