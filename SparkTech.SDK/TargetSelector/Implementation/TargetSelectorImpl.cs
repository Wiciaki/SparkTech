namespace SparkTech.SDK.TargetSelector.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.Properties;

    public class TargetSelectorImpl : ITargetSelector
    {
        public Menu Menu { get; }

        protected readonly List<Weight> weights;

        public TargetSelectorImpl()
        {
            this.Menu = new Menu("TargetSelector");

            this.weights = Weight.CreateWeights(this.Menu);
        }

        public virtual IHero GetTarget(IEnumerable<IHero> heroes)
        {
            var enemies = heroes.Where(hero => hero.IsValidTarget()).ToArray();

            if (enemies.Length == 0) return null;
            if (enemies.Length == 1) return enemies[0];

            var dictionary = enemies.ToDictionary(e => e, e => 0, new EntityComparer<IHero>());

            foreach (var (weight, value) in from weight in this.weights let v = weight.Value where v > 0 select (weight, v))
            {
                Array.Sort(enemies, weight);

                for (var i = dictionary.Count - 1; i >= 0; --i)
                {
                    dictionary[enemies[i]] += i * value;
                }
            }

            //Console.WriteLine("Weights: " + string.Join(", ", dictionary.Select(p => p.Value)));

            return dictionary.OrderByDescending(pair => pair.Value).First().Key;
        }

        public JObject GetTranslations()
        {
            return JObject.Parse(Resources.TargetSelector);
        }

        public void Start()
        { }

        public void Pause()
        { }
    }
}