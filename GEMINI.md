# {{CODENAME}} - Project Guidelines

## Project Overview
This project is built using Godot 4.6 and C# (.NET 8.0/9.0), following a data-driven, modding-first architecture.

## Folder Structure
- **`Art/`**: Source art files (Blender models, Krita/Photoshop files).
- **`Assets/`**: Generic project resources (Scenes, UI elements) that are needed during the initial boot sequence or are not part of a specific mod.
- **`Docs/`**: Project documentation.
- **`Mods/Core/`**: The primary game mod. This is where most game assets (textures, models, materials) and game definitions (JSON) should reside.
- **`Source/`**: C# source code, organized by namespace.

## Initialization Flow
1. **`Main.cs`**: The root node script that starts the boot sequence.
2. **`PreloaderTask`**: Initializes logging, settings, and the `ModManager`.
3. **`LoaderTask`**: Loads assets and definitions from all active mods.

## Modding & Data
- All game data should be defined as `Olib.Definitions.Definition` subclasses in `Source/{{CODENAME}}/Data`.
- Content is loaded from the `Mods/` directory. Each mod must have a `mod.json` and optionally an `assets.json` and a `Definitions/` folder.

## Getting Started
- Open the project in VS Code.
- Press **F5** to build and run the project via the Godot executable.
- Logs are written to `res://output.log` and the Godot console.
