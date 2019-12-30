namespace Surgical.SDK.Modules
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Surgical.SDK.Champion;
    using Surgical.SDK.Evade;
    using Surgical.SDK.HealthPrediction;
    using Surgical.SDK.Logging;
    using Surgical.SDK.MovementPrediction;
    using Surgical.SDK.Orbwalker;
    using Surgical.SDK.TargetSelector;

    public class ScriptLoader : IScriptLoader
    {
        public virtual void LoadFrom(string folder)
        {
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
            var modules = (from type in assembly.GetTypes()
                           where type.IsClass && !type.IsAbstract && typeof(IEntryPoint).IsAssignableFrom(type)
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