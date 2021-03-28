namespace SparkTech.SDK.Champion
{
    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.GUI.Menu;

    public class Jinx : IChampion
    {
        public void Start()
        {

        }

        public void Pause()
        {

        }

        public Menu Menu { get; } = new Menu("surgical");

        public JObject GetTranslations()
        {
            return null;
        }

        public float GetHealthIndicatorDamage(IHero hero)
        {
            return 0f;
        }
    }
}