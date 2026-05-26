# {{CODENAME}}

A Godot 4.6 C# project built with the Olib Template. This repository contains a standardized project structure and a modular, modding-first architecture.

## Features

- **Pipeline-Based Initialization**: A modular boot sequence using `InitializationContext` and `IInitStep`.
- **Threaded Asset Management**: Background loading for Scenes, Textures, Audio, Meshes, Fonts, and Shaders.
- **Integrated Modding**: Built-in support for `Olib.Modding` and `Olib.Definitions`.
- **VS Code Integration**: Pre-configured build tasks and F5 debugging for Godot.
- **Clean Architecture**: Decoupled initialization logic and data-driven content loading.

## Getting Started

### 1. Generating / Syncing (Generator Context)

If you are using this as a template to create or update a project:

**Generate a New Project:**
```bash
python create_project.py MyNewGame --author "Olib"
```

**Sync Template Updates:**
To pull the latest architecture updates into an existing project:
```bash
python create_project.py --sync MyNewGame
```
*Note: Syncing will overwrite files in the `Core/` folder and other system files.*

### 2. Open and Build

1. Open the project folder in **VS Code**.
2. Restore dependencies and build:
   ```bash
   dotnet build
   ```
3. Open the project in the **Godot Editor** to import assets for the first time.

### 3. Run and Debug

- Press **F5** in VS Code to build and run using the "Run Godot Run" profile.
- Logs are written to `res://output.log` and the Godot console.

## Project Structure

- **`Art/`**: Source art files (Blender, Krita, etc.).
- **`Assets/`**: Generic Godot resources (Scenes, UI) used before mods are loaded.
- **`Mods/Core/`**: The primary content mod. Contains `mod.json`, `assets.json`, and `Definitions/`.
- **`Source/{{CODENAME}}/`**: C# Source code.
  - **`Core/`**: The `PipelineRunner` and modular initialization steps.
  - **`Data/`**: `AssetManager`, `Setting` system, and `Olib.Definitions` implementations.

## Initialization Pipeline

The game starts in `Main.cs`, which runs an asynchronous `PipelineRunner` through these steps:

1. **`LoadSettings`**: Reads `res://settings.json`.
2. **`ModDiscovery`**: Finds and validates mods in `res://Mods`.
3. **`ModActivation`**: Activates the selected mods.
4. **`AssetLoading`**: Threaded background loading of assets defined in `assets.json`.
5. **`DefinitionProcessing`**: Parses JSON definitions into the registry.

## Development Guide

### Adding Assets
Place assets in `Mods/Core/` (or a new mod folder) and register them in the mod's `assets.json`.

### Adding Data
1. Create a C# class inheriting from `Olib.Definitions.Definition` in `Source/{{CODENAME}}/Data`.
2. Create a JSON file in `Mods/Core/Definitions/` using the new type.
3. The `ModDiscoveryStep` will automatically register the type via reflection.

## Prerequisites
- **Godot 4.6+ (.NET)**
- **.NET SDK 8.0/9.0**
- **Python 3.10+** (for the generator script)
