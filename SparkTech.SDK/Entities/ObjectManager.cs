namespace SparkTech.SDK.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using SparkTech.SDK.Logging;

    public static class ObjectManager
    {
        public static event Action<IGameObject> OnCreate, OnDelete;

        static ObjectManager()
        {
            var fragment = Platform.CoreFragment?.GetObjectManagerFragment() ?? throw Platform.FragmentException();

            fragment.Create = HandleCreate;
            fragment.Delete = HandleDelete;

            Array.ForEach(fragment.GetUnits(), HandleCreate);

            Player = GetById<IHero>(fragment.GetPlayerId());
        }

        public static IHero Player { get; }

        public static HashSet<TGameObject> Get<TGameObject>() where TGameObject : IGameObject
        {
            if (Container.TryGetValue(typeof(TGameObject), out var entry))
            {
                return (HashSet<TGameObject>)entry.HashSet;
            }

            Log.Info($"ObjectManager - now caching {typeof(TGameObject).Name}");

            var hashset = new HashSet<TGameObject>(Container[typeof(IGameObject)].HashSet.OfType<TGameObject>(), new EntityComparer<TGameObject>());
            Container.Add(typeof(TGameObject), new CacheEntry(hashset));

            return hashset;
        }

        public static IGameObject GetById(int id)
        {
            return GetById<IGameObject>(id);
        }

        public static TGameObject GetById<TGameObject>(int id) where TGameObject : IGameObject
        {
            return Get<TGameObject>().FirstOrDefault(o => o.Id == id);
        }

        private static void HandleCreate(IGameObject sender) => ProcessItem(sender, true);

        private static void HandleDelete(IGameObject sender) => ProcessItem(sender, false);

        private static readonly Dictionary<Type, CacheEntry> Container = new Dictionary<Type, CacheEntry>
        {
            [typeof(IGameObject)] = new CacheEntry(new HashSet<IGameObject>(new EntityComparer<IGameObject>()))
        };

        private static readonly object[] ArgsHelperArray = new object[1];

        private static void ProcessItem(IGameObject sender, bool add)
        {
            lock (Container)
            {
                var senderType = sender.GetType();

                foreach (var entry in Container.Where(p => p.Key.IsAssignableFrom(senderType)).Select(p => p.Value))
                {
                    MethodBase method;
                    Action<IGameObject> action;

                    if (add)
                    {
                        method = entry.AddMethod;
                        action = OnCreate;
                    }
                    else
                    {
                        method = entry.RemoveMethod;
                        action = OnDelete;
                    }

                    ArgsHelperArray[0] = sender;
                    method.Invoke(entry.HashSet, ArgsHelperArray);
                    action.SafeInvoke(sender);
                }
            }
        }

        private class CacheEntry
        {
            #region Fields

            public readonly IEnumerable HashSet;

            public readonly MethodBase AddMethod;

            public readonly MethodBase RemoveMethod;

            #endregion

            #region Constructors and Destructors

            public CacheEntry(IEnumerable hashSet)
            {
                this.HashSet = hashSet;

                var type = hashSet.GetType();

                this.AddMethod = type.GetMethod("Add");
                this.RemoveMethod = type.GetMethod("Remove");
            }

            #endregion
        }
    }
}