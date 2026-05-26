using System.Collections.Generic;
using {{CODENAME}}.Data;
using Olib.Modding;

namespace {{CODENAME}}.Core
{
    public class InitializationContext
    {
        public Setting Settings { get; set; }
        public ModManager ModManager { get; set; }
        public AssetManager AssetManager { get; set; }
        
        // Track errors for Safe Mode / Recovery
        public List<string> Errors { get; } = new List<string>();
        public bool IsCriticalFailure { get; set; }

        public void ReportError(string error, bool critical = false)
        {
            Errors.Add(error);
            if (critical) IsCriticalFailure = true;
        }
    }
}
