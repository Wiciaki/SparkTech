namespace Surgical.SDK.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Surgical.SDK.Champion;
    using Surgical.SDK.Evade;
    using Surgical.SDK.GUI.Notifications;
    using Surgical.SDK.HealthPrediction;
    using Surgical.SDK.Logging;
    using Surgical.SDK.MovementPrediction;
    using Surgical.SDK.Orbwalker;
    using Surgical.SDK.TargetSelector;

    public class Loader
    {
        protected virtual Assembly LoadAssembly(string path)
        {
            return Assembly.LoadFrom(path);
        }

        private static readonly HashSet<string> Loaded = new HashSet<string>();

        private static bool canWarn;

        static Loader()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        public void LoadAll()
        {
            var files = Array.FindAll(Directory.GetFiles(Folder.Scripts), p => Path.GetExtension(p) == ".dll" && !Loaded.Contains(p));

            if (files.Length > 0)
            {
                Parallel.ForEach(files, this.LoadFrom);
            }
            else if (canWarn)
            {
                var content = SdkSetup.GetString("loaderContent")!;
                var header = SdkSetup.GetString("loaderHeader")!;

                Notification.Send(content, header);
            }

            canWarn = true;
        }

        public void LoadFrom(string path)
        {
            if (!Loaded.Add(path))
            {
                return;
            }

            var name = Path.GetFileName(path);

            Log.Info($"Loading {name}...");

            Assembly assembly;

            try
            {
                assembly = this.LoadAssembly(path);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return;
            }

            ProcessAssembly(assembly);

            SdkSetup.SetScriptsCount(Loaded.Count);
            Notification.Send($"Surgical.SDK: Loaded {name}!");
        }

        private static void ProcessAssembly(Assembly assembly)
        {
            var modules = (from type in assembly.GetTypes()
                           where type.IsClass && typeof(IEntryPoint).IsAssignableFrom(type)
                           let constructor = type.GetConstructor(Type.EmptyTypes)
                           where constructor != null && constructor.IsPublic
                           let name = type.Name
                           let module = DynamicEntryPoint<IModule>(constructor, name)
                           where module != null
                           orderby name
                           select module).ToArray();

            AddModulesTo(TargetSelectorService.Picker);
            AddModulesTo(HealthPredictionService.Picker);
            AddModulesTo(MovementPredictionService.Picker);
            AddModulesTo(EvadeService.Picker);
            AddModulesTo(OrbwalkerService.Picker);
            AddModulesTo(ChampionService.Picker);

            void AddModulesTo<T>(Picker<T> picker) where T : class, IModule
            {
                foreach (var module in modules.OfType<T>())
                {
                    picker.Add(module);
                }
            }

            static T? DynamicEntryPoint<T>(ConstructorInfo constructor, string typeName) where T : class, IEntryPoint
            {
                object o;

                try
                {
                    o = constructor.Invoke(Array.Empty<object>());
                }
                catch (Exception ex)
                {
                    Log.Error($"Couldn't create a new instance of {typeName}! Problem executing the parameterless constructor");
                    Log.Error(ex);

                    return null;
                }

                Log.Info($"Instantiated {typeName}!");

                return o as T;
            }
        }
    }
}