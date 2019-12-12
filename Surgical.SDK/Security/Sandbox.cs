namespace Surgical.SDK.Security
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Surgical.SDK.Champion;
    using Surgical.SDK.Evade;
    using Surgical.SDK.HealthPrediction;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Modules;
    using Surgical.SDK.MovementPrediction;
    using Surgical.SDK.Orbwalker;
    using Surgical.SDK.TargetSelector;

    public class Sandbox : ISandbox
    {
        public virtual void LoadScripts()
        {
            var folder = Folder.Root.GetFolder("ThirdParty");
            var files = Directory.EnumerateFiles(folder).Where(path => Path.GetExtension(path) == ".dll");

            foreach (var path in files)
            {
                Log.Info($"Loading {Path.GetFileName(path)}...");

                this.ProcessAssembly(Assembly.LoadFrom(path));
            }
        }

        protected virtual void ProcessAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();

            AddTo(TargetSelectorService.Picker);
            AddTo(HealthPredictionService.Picker);
            AddTo(MovementPredictionService.Picker);
            AddTo(EvadeService.Picker);
            AddTo(OrbwalkerService.Picker);
            AddTo(ChampionService.Picker);

            void AddTo<T>(Picker<T> picker) where T : class, IModule
            {
                foreach (var type in types.Where(typeof(T).IsAssignableFrom))
                {
                    T module;

                    try
                    {
                        module = (T)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Couldn't create a new instance of {typeof(T).Name} module from {assembly.GetName().Name}");
                        Log.Error("Problem launching the parameterless .ctor");
                        Log.Error(ex);
                        continue;
                    }

                    picker.Add(module);
                }
            }
        }
    }
}