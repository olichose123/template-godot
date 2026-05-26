using System.Collections.Generic;
using System.Text.Json;
using Chickensoft.Log;
using Chickensoft.Log.Godot;
using Godot;
using Olib.Modding;

namespace {{CODENAME}}.Data
{
    public class Setting
    {
        static ILog Log = new Log(nameof(Setting), new GDWriter(), GDFileWriter.Instance());

        public List<ActiveMod> Mods { get; set; } = new List<ActiveMod>();
        public List<string> ModDirectories { get; set; } = new List<string>{
            "res://Mods"
        };

        public static Setting Load(string path)
        {
            if (!FileAccess.FileExists(path))
            {
                Log.Print($"Settings file not found at {path}. Creating default.");
                return new Setting();
            }

            FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                return new Setting();
            }

            string json = file.GetAsText();
            return JsonSerializer.Deserialize<Setting>(json, new JsonSerializerOptions
            {
                Converters = { new JsonSemverConverter() }
            });
        }

        public void Save(string path)
        {
            using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            file.StoreString(JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonSemverConverter() }
            }));
        }
    }
}
