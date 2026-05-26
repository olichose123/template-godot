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

            // 5. Validate (but don't activate yet)
            var report = context.ModManager.ValidateActiveMods(context.Settings.Mods);
            foreach (var error in report)
            {
                context.ReportError($"Mod validation: {error}");
            }

            return Task.CompletedTask;
        }
    }
}
