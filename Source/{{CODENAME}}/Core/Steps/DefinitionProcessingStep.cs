using System.Threading.Tasks;

namespace {{CODENAME}}.Core.Steps
{
    public class DefinitionProcessingStep : IInitStep
    {
        public string Description => "Processing Definitions";

        public Task Execute(InitializationContext context)
        {
            context.AssetManager.ProcessDefinitions();
            return Task.CompletedTask;
        }
    }
}
