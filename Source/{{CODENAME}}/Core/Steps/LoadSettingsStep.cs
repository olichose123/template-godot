using System.Threading.Tasks;
using {{CODENAME}}.Data;

namespace {{CODENAME}}.Core.Steps
{
    public class LoadSettingsStep : IInitStep
    {
        public string Description => "Loading Settings";

        public Task Execute(InitializationContext context)
        {
            context.Settings = Setting.Load(Game.SettingsPath);
            context.Settings.Save(Game.SettingsPath); // Ensure default file exists
            return Task.CompletedTask;
        }
    }
}
