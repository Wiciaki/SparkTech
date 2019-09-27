namespace Surgical.SDK.TargetSelector.Default
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;

    public class TargetSelector : ITargetSelector
    {
        public Menu Menu { get; }

        private readonly List<Weight> Weights;

        private readonly IEqualityComparer<IHero> EqualityComparer;

        public TargetSelector()
        { 
            // var modeNames = new[] { "timeToKill", "dealsMostDmg", "distanceChamp", "distanceMouse" };

            EqualityComparer = new EntityComparer<IHero>();
            Weights = new List<Weight>();

            Menu = new Menu("TargetSelector")
                   {
                       new MenuText("Notice")
                   };

            // TODO: other weights
            //Register<DistanceWeight>();

            //void Register<TWeight>() where TWeight : Weight, new()
            //{
            //    var weight = new TWeight();
                
            //    Weights.Add(weight);

            //    foreach (var component in weight.CreateItems())
            //    {
            //        Menu.Add(component);
            //    }
            //}
        }

        IHero ITargetSelector.Select(IEnumerable<IHero> heroes)
        {
            var enemies = heroes.Where(hero => hero.IsEnemy()).Distinct(EqualityComparer).ToList();

            if (enemies.Count == 0)
            {
                return null;
            }

            var sortedEnemies = enemies.ToArray();
            var weightCollection = new Dictionary<IHero, int>(enemies.Count, EqualityComparer);
            enemies.ForEach(enemy => weightCollection.Add(enemy, 0));

            foreach (var weight in Weights)
            {
                var w = weight.Importance;

                if (w == 0)
                {
                    continue;
                }

                Array.Sort(sortedEnemies, weight);

                for (var i = weightCollection.Count; i > 0; --i)
                {
                    weightCollection[sortedEnemies[i]] += i * w;
                }
            }

            return weightCollection.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).First();
        }

        public JObject GetTranslations()
        {
            return null;
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}