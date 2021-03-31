namespace SparkTech.SDK.DamageLibrary.Implementation
{
    using Newtonsoft.Json.Linq;

    using GUI.Menu;
    using SparkTech.SDK.Entities;

    public class DamageLibraryImpl : IDamageLibrary
    {
        public Menu Menu { get; }

        public JObject GetTranslations()
        {
            return null;
        }

        public void Start()
        { }

        public void Pause()
        { }

        public float GetAutoAttackDamage(IUnit source, IUnit target, bool includePassive)
        {
            throw new System.NotImplementedException();
        }

        public float GetSpellDamage(IHero source, IUnit target, SpellSlot slot, int stage)
        {
            throw new System.NotImplementedException();
        }
    }
}