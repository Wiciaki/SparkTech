namespace SparkTech.Ensoul.Ports
{
    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.Orbwalker;

    class EnsoulOrbwalker : Orbwalker
    {
        public override Menu Menu { get; } = new Menu("Ensoul");

        public override JObject GetTranslations()
        {
            return null;
        }

        public override void Start()
        {
            Program.SetEnsoulOrbwalker(true);
        }

        public override void Pause()
        {
            Program.SetEnsoulOrbwalker(false);
        }
    }
}