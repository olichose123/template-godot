using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Olib.Definitions;
using Olib.Modding;
using {{CODENAME}}.Data;

namespace {{CODENAME}}.Core.Steps
{
    public class ModDiscoveryStep : IInitStep
    {
        public string Description => "Discovering and Validating Mods";

        public Task Execute(InitializationContext context)
        {
            // 1. Resolve definition types (Discovery)
            List<Type> definitionTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Definition)))
                .ToList();
            DefinitionTypeResolver.DerivedTypes = definitionTypes;

            // 2. Add converters
            Definition.JsonSerializerOptions.Converters.Add(new JsonSemverConverter());
            Definition.JsonSerializerOptions.Converters.Add(new JsonSemverRangeConverter());
            Definition.JsonSerializerOptions.Converters.Add(new ResourceReferenceJsonConverter());
            Definition.JsonSerializerOptions.Converters.Add(new ReferenceJsonConverter());

            // 3. Initialize managers
            context.AssetManager = new AssetManager();
            context.ModManager = new ModManager(context.Settings.ModDirectories);

            // 4. Find mods
            context.ModManager.FindMods(new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonSemverConverter(), new JsonSemverRangeConverter() }
            });

            // 5. Validate active mods (Presence and Strict Load Order)
            var processedModNames = new HashSet<string>();
            foreach (var requestedMod in context.Settings.Mods)
            {
                // Find the mod in available mods
                var mod = context.ModManager.AvailableMods.FirstOrDefault(m => 
                    m.Name == requestedMod.Name && m.Version == requestedMod.Version);

                if (mod == null)
                {
                    context.ReportError($"Active mod not found: {requestedMod.Name} v{requestedMod.Version}", true);
                    continue;
                }

                // Check if all dependencies are already in the load order (Strict Order)
                foreach (var dependency in mod.Dependencies)
                {
                    if (!processedModNames.Contains(dependency.Name))
                    {
                        // Check if it's even in the total active list (it might be missing entirely or just late)
                        bool isPresentLater = context.Settings.Mods.Any(m => m.Name == dependency.Name);
                        
                        if (isPresentLater)
                        {
                            context.ReportError($"Load Order Error: Mod '{mod.Name}' depends on '{dependency.Name}', but '{dependency.Name}' is listed AFTER it in settings.json. Move '{dependency.Name}' up.", true);
                        }
                        else
                        {
                            context.ReportError($"Missing Dependency: Mod '{mod.Name}' requires '{dependency.Name}' ({dependency.CompatibleVersions}), but it is not active.", true);
                        }
                    }
                    else
                    {
                        // Dependency is present and earlier - check version compatibility
                        var depMod = context.ModManager.AvailableMods.First(m => m.Name == dependency.Name); 
                        if (!dependency.IsCompatible(depMod))
                        {
                            context.ReportError($"Incompatible Dependency: Mod '{mod.Name}' requires '{dependency.Name}' {dependency.CompatibleVersions}, but version {depMod.Version} is loaded.", true);
                        }
                    }
                }

                processedModNames.Add(mod.Name);
            }

            return Task.CompletedTask;
        }
    }
}
