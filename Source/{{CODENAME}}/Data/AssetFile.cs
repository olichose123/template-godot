using System.Collections.Generic;
using System.Text.Json;
using Chickensoft.Log;
using Chickensoft.Log.Godot;
using Godot;

namespace {{CODENAME}}.Data
{
    public class AssetFile
    {
        static ILog Log = new Log(nameof(AssetFile), new GDWriter(), GDFileWriter.Instance());

        public Dictionary<string, string> Textures { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Scenes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Audio { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Meshes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Fonts { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Shaders { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Materials { get; set; } = new Dictionary<string, string>();
        
        public List<string> DefinitionDirectories { get; set; } = new List<string>();

        public static AssetFile Load(string path)
        {
            if (!FileAccess.FileExists(path))
            {
                Log.Print($"Asset file not found at {path}.");
                return null;
            }

            FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                Log.Err($"Failed to open asset file at {path}.");
                return null;
            }

            string json = file.GetAsText();
            return JsonSerializer.Deserialize<AssetFile>(json);
        }

        public void Save(string path)
        {
            using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            file.StoreString(JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
