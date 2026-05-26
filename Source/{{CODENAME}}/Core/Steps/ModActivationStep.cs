using System.Threading.Tasks;

namespace {{CODENAME}}.Core.Steps
{
    public class ModActivationStep : IInitStep
    {
        public string Description => "Activating Mods";

        public Task Execute(InitializationContext context)
        {
            context.ModManager.ActivateMods(context.Settings.Mods);
            return Task.CompletedTask;
        }
    }
}
