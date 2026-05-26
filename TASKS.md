# Godot Template Roadmap & Tasks

This file tracks planned features, improvements, and bug fixes for the Godot C# project template.

## High Priority: Robustness & Initialization
- [ ] **Sub-step Progress Reporting**: Update `IInitStep` and `PipelineRunner` to support granular progress reporting (e.g., 0-100% within `AssetLoadingStep`).
- [ ] **Safe Mode / Recovery UI**: Implement a minimal Godot scene in `Assets/` that displays critical initialization errors and allows the user to reset settings or disable mods.
- [ ] **Default Loading Screen**: Create a base loading screen UI in `Assets/` that automatically connects to the `PipelineRunner.ProgressChanged` event.
- [ ] **Advanced Error Logging**: Enhance the `InitializationContext` to include stack traces and mod-specific error attribution.

## Medium Priority: Modding & Data
- [ ] **Definition Registry & Validation**: 
    - [ ] Create a central registry for parsed definitions.
    - [ ] Add a `DefinitionValidationStep` to verify that all `ResourceReference` and cross-references (IDs) are valid after loading.
- [ ] **Mod Dependency Graph**: Extend `ModDiscoveryStep` to resolve complex mod dependency trees (e.g., Mod A requires Mod B > 1.2.0).
- [ ] **Definition Hot-Reloading**: Add support for re-parsing JSON definitions in-game for faster data iteration.
- [ ] **Localization System**: Implement a mod-aware localization step to load translation files (`.po` or JSON) from active mods.

## Low Priority: Tooling & Architecture
- [ ] **Parallel Pipeline Execution**: Refactor `PipelineRunner` to allow independent steps (like settings load and mod discovery) to run concurrently.
- [ ] **Asset Manager Refresh**: Add a method to unload/reload assets for specific mods without restarting the game.
- [ ] **CI/CD Integration**: Add a GitHub Actions template for automated `dotnet build` and Godot export.
- [ ] **Unit Test Template**: Add a pre-configured `Chickensoft.GoDotTest` setup to the template.

## Completed Tasks
- [x] Initial Pipeline-based architecture.
- [x] Threaded background asset loading.
- [x] Expanded asset support (Scenes, Audio, Meshes, etc.).
- [x] Mod-driven data discovery via reflection.
- [x] Python/Poetry project generation script.
- [x] Documentation (READMEs).
