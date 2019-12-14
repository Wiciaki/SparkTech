namespace Surgical.SDK.Security
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

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

            Parallel.ForEach(files, LoadFile);

            void LoadFile(string path)
            {
                Log.Info($"Loading {Path.GetFileName(path)}...");

                Assembly assembly;

                try
                {
                    assembly = Assembly.LoadFrom(path);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return;
                }

                this.ProcessAssembly(assembly);
            }
        }

        protected virtual void ProcessAssembly(Assembly assembly)
        {
            var constructors = from type in assembly.GetTypes()
                               where type.IsClass && !type.IsAbstract && typeof(IEntryPoint).IsAssignableFrom(type)
                               orderby type.Name
                               let ctor = type.GetConstructor(Type.EmptyTypes)
                               where ctor != null && ctor.IsPublic
                               select ctor;

            var modules = new List<IModule>();

            foreach (var constructor in constructors)
            {
                // ReSharper disable once PossibleNullReferenceException
                var typeName = constructor.DeclaringType.FullName;

                try
                {
                    if (constructor.Invoke(Array.Empty<object>()) is IModule module)
                    {
                        modules.Add(module);
                    }

                    Log.Info($"Instantiated {typeName}!");
                }
                catch (Exception ex)
                {
                    Log.Error($"Couldn't create a new instance of {typeName}! Problem executing the parameterless constructor");
                    Log.Error(ex);
                }
            }

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
        }
    }
}