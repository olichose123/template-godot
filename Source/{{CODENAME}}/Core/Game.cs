using Godot;
using {{CODENAME}}.Data;
using Olib.Modding;

namespace {{CODENAME}}.Core
{
    public static class Game
    {
        public static Node Root { get; private set; }
        public static AssetManager AssetManager { get; set; }
        public static ModManager ModManager { get; set; }
        public static Setting Settings { get; set; }

        public static string SettingsPath => "res://settings.json";

        public static void Initialize(Node root)
        {
            Root = root;
        }

        public static void Update(float delta)
        {
            // Global update logic
        }

        public static void UpdateLoadingScreen(float progress)
        {
            // Implement loading screen update logic
        }

        public static void ShowErrorScreen(System.Collections.Generic.List<string> errors)
        {
            // Implement error screen logic
        }

        public static void StartMainMenu()
        {
            // Implement main menu transition
        }
    }
}
