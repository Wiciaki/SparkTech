//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 27/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  ObjectCache.cs is a part of SparkTech
//
//  SparkTech is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  SparkTech is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  You should have received a copy of the GNU General Public License
//  along with SparkTech. If not, see <http://www.gnu.org/licenses/>.
//
//  -------------------------------------------------------------------

namespace SparkTech.SDK.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Platform.API;

    public static class ObjectManager
    {
        private static readonly Dictionary<Type, CacheEntry> Container = new Dictionary<Type, CacheEntry>
                                                                         {
                                                                             [typeof(IGameObject)] = new CacheEntry(
                                                                                 new HashSet<IGameObject>(
                                                                                     new GameObjectComparer()))
                                                                         };

        public static event Action<IGameObject> OnCreate, OnDelete;

        internal static void Initialize(IObjectManager mgr)
        {
            Player = mgr.GetPlayer();

            mgr.Create = HandleCreate;
            mgr.Delete = HandleDelete;

            Array.ForEach(mgr.GetUnits(), HandleCreate);
        }

        public static IPlayer Player { get; private set; }

        public static HashSet<TGameObject> Get<TGameObject>() where TGameObject : IGameObject
        {
            if (Container.TryGetValue(typeof(TGameObject), out var entry))
            {
                return (HashSet<TGameObject>)entry.HashSet;
            }

            Log.Info($"ObjectManager - now caching {typeof(TGameObject).Name}!");

            var hashset = Container[typeof(IGameObject)].HashSet.OfType<TGameObject>().ToHashSet(new EntityComparer<TGameObject>());

            Container.Add(typeof(TGameObject), new CacheEntry(hashset));

            return hashset;
        }

        public static bool IsMe(this IGameObject o) => o.Id() == Player.Id();

        public static bool IsAlly(this IGameObject o) => o.Team() == Player.Team();

        public static bool IsEnemy(this IGameObject o) => !o.IsAlly();

        public static IGameObject GetById(int id) => Get<IGameObject>().FirstOrDefault(o => o.Id() == id);

        private static void HandleCreate(IGameObject sender) => ProcessItem(sender, true);

        private static void HandleDelete(IGameObject sender) => ProcessItem(sender, false);

        private static readonly object[] Args = new object[1];

        private static void ProcessItem(IGameObject sender, bool add)
        {
            var senderType = sender.GetType();

            foreach (var (_, entry) in Container.Where(p => p.Key.IsAssignableFrom(senderType)))
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

                Args[0] = sender;

                method.Invoke(entry.HashSet, Args);
                action.SafeInvoke(sender);
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
                var type = hashSet.GetType();

                this.AddMethod = type.GetMethod("Add");

                this.RemoveMethod = type.GetMethod("Remove");

                this.HashSet = hashSet;
            }

            #endregion
        }
    }
}