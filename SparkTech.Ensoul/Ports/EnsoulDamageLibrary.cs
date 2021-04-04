namespace SparkTech.Ensoul.Ports
{
    using Newtonsoft.Json.Linq;

    using SDK.DamageLibrary;
    using SDK.GUI.Menu;

    using SparkTech.SDK.Entities;

    public class EnsoulDamageLibrary : IDamageLibrary
    {
        public Menu Menu { get; } = new Menu("Ensoul Port");

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
            var s = EnsoulSharp.ObjectManager.GetUnitByNetworkId<EnsoulSharp.AIBaseClient>(source.Id);
            var t = EnsoulSharp.ObjectManager.GetUnitByNetworkId<EnsoulSharp.AIBaseClient>(target.Id);

            return (float)EnsoulSharp.SDK.Damage.GetAutoAttackDamage(s, t, includePassive);
        }

        public float GetSpellDamage(IHero source, IUnit target, SpellSlot slot, int stage)
        {
            var s = EnsoulSharp.ObjectManager.GetUnitByNetworkId<EnsoulSharp.AIHeroClient>(source.Id);
            var t = EnsoulSharp.ObjectManager.GetUnitByNetworkId<EnsoulSharp.AIBaseClient>(target.Id);

            return (float)EnsoulSharp.SDK.Damage.GetSpellDamage(s, t, (EnsoulSharp.SpellSlot)slot, stage);
        }
    }
}