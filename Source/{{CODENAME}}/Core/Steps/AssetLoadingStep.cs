using System.Threading.Tasks;
using {{CODENAME}}.Data;

namespace {{CODENAME}}.Core.Steps
{
    public class AssetLoadingStep : IInitStep
    {
        public string Description => "Loading Assets";

        public async Task Execute(InitializationContext context)
        {
            // 1. Queue all assets from active mods
            foreach (var mod in context.ModManager.ActiveMods)
            {
                var assetFilePath = mod.DirectoryPath + "/assets.json";
                var assetFile = AssetFile.Load(assetFilePath);
                if (assetFile != null)
                {
                    context.AssetManager.LoadAssets(mod.DirectoryPath, assetFile);
                }
            }

            // 2. Wait for threaded loading to complete
            await context.AssetManager.WaitForPendingLoads(progress => 
            {
                // You could report sub-progress here if the pipeline supported it
            });
        }
    }
}
