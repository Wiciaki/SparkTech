namespace SparkTech
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using MoreLinq;

    using SparkTech.Caching;
    using SparkTech.Utils;

    public static class ObjectManager
    {
        #region Static Fields

        private static readonly IEqualityComparer<IGameObject> Comparer;

        private static readonly Dictionary<Type, ObjectCacheEntry> Container;

        private static readonly Dictionary<GameObjectTeam, ObjectTeam> TeamDictionary;

        #endregion

        #region Constructors and Destructors

        static ObjectManager()
        {
            //if (!GameLoading.IsLoaded)
            //{
            //    Logging.Log("Don't use ObjectCache before the game is loaded!");
            //}

            Logging.Log("ObjectCache - starting!");

            var aT = Player.Team;

            TeamDictionary = new Dictionary<GameObjectTeam, ObjectTeam>(4)
                                 {
                                     [aT] = ObjectTeam.Ally,
                                     [aT == GameObjectTeam.Order ? GameObjectTeam.Chaos : GameObjectTeam.Order] =
                                         ObjectTeam.Enemy,
                                     [GameObjectTeam.Neutral] = ObjectTeam.Neutral,
                                     [GameObjectTeam.Unknown] = ObjectTeam.Unknown
                                 };

            Comparer = new GameObjectComparer();

            GameObject.OnCreate += args => ProcessItem(args.Sender, true);
            GameObject.OnDelete += args => ProcessItem(args.Sender, false);

            Container = new Dictionary<Type, ObjectCacheEntry>
                            {
                                [typeof(GameObject)] = new ObjectCacheEntry(new HashSet<GameObject>(Comparer))
                            };

            Array.ForEach(ObjectManager.ObjectsUncached, o => ProcessItem(o, true));
        }

        #endregion

        public static IPlayer Player { get; private set; }

        #region Public Methods and Operators

        public static HashSet<TGameObject> Get<TGameObject>()
            where TGameObject : GameObject
        {
            if (Container.TryGetValue(typeof(TGameObject), out var entry))
            {
                return (HashSet<TGameObject>)entry.HashSet;
            }

            Logging.Log($"ObjectCache - Adding {typeof(TGameObject).Name}!");

            var hashset = new HashSet<TGameObject>(
                Container[typeof(GameObject)].HashSet.OfType<TGameObject>(),
                Comparer);

            Container.Add(typeof(TGameObject), new ObjectCacheEntry(hashset));

            return hashset;
        }

        /// <summary>
        ///     Gets the <see cref="ObjectTeam" /> representation of the current object
        /// </summary>
        /// <param name="object">The <see cref="GameObject" /> to be inspected</param>
        /// <returns></returns>
        public static ObjectTeam Team(this GameObject @object) => @object.Team.ToObjectTeam();

        /// <summary>
        ///     Gets the <see cref="ObjectTeam" /> representation of the current object
        /// </summary>
        /// <param name="team">The <see cref="GameObjectTeam" /> instance to be converted</param>
        /// <returns></returns>
        public static ObjectTeam ToObjectTeam(this GameObjectTeam team) => TeamDictionary[team];

        #endregion

        #region Methods

        private static void ProcessItem(IGameObject sender, bool @new)
        {
            var senderType = sender.GetType();

            (from p in Container where p.Key.IsAssignableFrom(senderType) select p.Value).ForEach(v => v.Process(sender, @new));
        }

        #endregion

        private class ObjectCacheEntry
        {
            #region Fields

            public readonly IEnumerable HashSet;

            private readonly MethodInfo add;

            private readonly MethodInfo remove;

            private readonly object[] args;

            #endregion

            #region Constructors and Destructors

            public ObjectCacheEntry(IEnumerable hashSet)
            {
                var type = hashSet.GetType();

                this.add = type.GetMethod("Add");

                this.remove = type.GetMethod("Remove");

                this.args = new object[1];

                this.HashSet = hashSet;
            }

            #endregion

            #region Public Methods and Operators

            public void Process(IGameObject o, bool @new)
            {
                this.args[0] = o;

                (@new ? this.add : this.remove).Invoke(this.HashSet, this.args);
            }

            #endregion
        }
    }
}