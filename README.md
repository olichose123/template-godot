# {{CODENAME}}

A Godot 4.6 C# project built with the Olib Template.

## Project Structure

- **`Art/`**: High-level source files (Blender, Photoshop, etc.).
- **`Assets/`**: Generic Godot resources (Scenes, generic UI) used before mods are loaded.
- **`Mods/Core/`**: The primary content mod. Contains `mod.json`, `assets.json`, and the `Definitions/` folder.
- **`Source/{{CODENAME}}/`**: C# Source code.
  - **`Core/`**: Contains the `PipelineRunner` and the modular initialization steps.
  - **`Data/`**: Contains the `AssetManager`, `Setting` system, and `Olib.Definitions` implementations.

## Initialization Pipeline

The game starts in `Main.cs`, which runs an asynchronous `PipelineRunner`. The pipeline follows these steps:

1. **`LoadSettings`**: Reads `res://settings.json`.
2. **`ModDiscovery`**: Finds and validates mods in the `res://Mods` directory.
3. **`ModActivation`**: Activates the selected mods.
4. **`AssetLoading`**: Performs **threaded background loading** of all assets defined in the mods' `assets.json`.
5. **`DefinitionProcessing`**: Parses JSON definitions into the `Olib.Definitions` registry.

## Development

### Adding Assets
Assets should be placed in `Mods/Core/` (or a new mod folder) and registered in that mod's `assets.json`.

### Adding Data
1. Create a C# class inheriting from `Olib.Definitions.Definition` in `Source/{{CODENAME}}/Data`.
2. Create a JSON file in `Mods/Core/Definitions/` using the new type.
3. The `ModDiscoveryStep` automatically registers your type via reflection.

### Debugging
- Press **F5** in VS Code to build and run.
- Use `Chickensoft.Log` for logging. Output is mirrored to `res://output.log`.

## Prerequisites
- Godot 4.6 (.NET)
- .NET SDK 8.0/9.0
