namespace SparkTech.SDK.TargetSelection.Default
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.Modules;
    using SparkTech.SDK.TargetSelection.Default.Weights;

    internal sealed class DefaultTargetSelector : ITargetSelector
    {
        Menu IModule.Menu => Menu;

        private static readonly Menu Menu;

        private static readonly List<Weight> Weights;

        private static readonly IEqualityComparer<IHero> EqualityComparer;

        static DefaultTargetSelector()
        { 
            // var modeNames = new[] { "timeToKill", "dealsMostDmg", "distanceChamp", "distanceMouse" };

            EqualityComparer = new EntityComparer<IGameObject>();
            Weights = new List<Weight>();

            Menu = new Menu("TargetSelector")
                   {
                       new MenuText("Notice")
                   };

            // TODO: other weights
            Register<DistanceWeight>();

            static void Register<TWeight>() where TWeight : Weight, new()
            {
                var weight = new TWeight();
                
                Weights.Add(weight);

                foreach (var component in weight.CreateItems())
                {
                    Menu.Add(component);
                }
            }
        }

        IHero ITargetSelector.SelectTarget(IEnumerable<IHero> heroes)
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
                var w = weight.GetWeight();

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

        void IModule.Release()
        {

        }
    }
}