using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chickensoft.Log;
using Chickensoft.Log.Godot;
using Godot;
using Olib.Definitions;

namespace {{CODENAME}}.Data
{
    public class AssetManager
    {
        ILog Log = new Log(nameof(AssetManager), new GDWriter(), GDFileWriter.Instance());

        private readonly Dictionary<Type, Dictionary<string, Resource>> _registry = new Dictionary<Type, Dictionary<string, Resource>>();
        public Dictionary<string, string> DefinitionJsons { get; private set; } = new Dictionary<string, string>();
        
        private readonly Dictionary<string, (string name, Type type)> _pendingThreadedLoads = new Dictionary<string, (string, Type)>();

        public AssetManager()
        {
            // Pre-initialize common registries
            _registry[typeof(Texture2D)] = new Dictionary<string, Resource>();
            _registry[typeof(PackedScene)] = new Dictionary<string, Resource>();
            _registry[typeof(AudioStream)] = new Dictionary<string, Resource>();
            _registry[typeof(Mesh)] = new Dictionary<string, Resource>();
            _registry[typeof(Font)] = new Dictionary<string, Resource>();
            _registry[typeof(Shader)] = new Dictionary<string, Resource>();
            _registry[typeof(Material)] = new Dictionary<string, Resource>();
        }

        public void LoadAssets(string path, AssetFile assetFile)
        {
            foreach (var kvp in assetFile.Textures) QueueThreadedLoad(kvp.Key, path + kvp.Value, typeof(Texture2D));
            foreach (var kvp in assetFile.Scenes) QueueThreadedLoad(kvp.Key, path + kvp.Value, typeof(PackedScene));
            foreach (var kvp in assetFile.Audio) QueueThreadedLoad(kvp.Key, path + kvp.Value, typeof(AudioStream));
            foreach (var kvp in assetFile.Meshes) QueueThreadedLoad(kvp.Key, path + kvp.Value, typeof(Mesh));
            foreach (var kvp in assetFile.Fonts) QueueThreadedLoad(kvp.Key, path + kvp.Value, typeof(Font));
            foreach (var kvp in assetFile.Shaders) QueueThreadedLoad(kvp.Key, path + kvp.Value, typeof(Shader));
            foreach (var kvp in assetFile.Materials) QueueThreadedLoad(kvp.Key, path + kvp.Value, typeof(Material));

            foreach (var dir in assetFile.DefinitionDirectories)
            {
                LoadDefinitionDirectory(path + "/" + dir);
            }
        }

        private void QueueThreadedLoad(string name, string path, Type type)
        {
            var error = ResourceLoader.LoadThreadedRequest(path, type.Name);
            if (error == Error.Ok)
            {
                _pendingThreadedLoads[path] = (name, type);
            }
            else
            {
                Log.Err($"Failed to start threaded load for {type.Name}: {path} (Error: {error})");
            }
        }

        public async Task WaitForPendingLoads(Action<float> onProgress)
        {
            var total = _pendingThreadedLoads.Count;
            if (total == 0) return;

            while (_pendingThreadedLoads.Count > 0)
            {
                var completed = new List<string>();
                foreach (var path in _pendingThreadedLoads.Keys)
                {
                    var status = ResourceLoader.LoadThreadedGetStatus(path);
                    if (status == ResourceLoader.ThreadLoadStatus.Loaded)
                    {
                        var info = _pendingThreadedLoads[path];
                        var resource = ResourceLoader.LoadThreadedGet(path);
                        
                        if (!_registry.ContainsKey(info.type)) _registry[info.type] = new Dictionary<string, Resource>();
                        _registry[info.type][info.name] = resource;
                        
                        completed.Add(path);
                    }
                    else if (status == ResourceLoader.ThreadLoadStatus.Failed || status == ResourceLoader.ThreadLoadStatus.InvalidResource)
                    {
                        Log.Err($"Threaded load failed for: {path}");
                        completed.Add(path);
                    }
                }

                foreach (var path in completed) _pendingThreadedLoads.Remove(path);

                float progress = 1.0f - ((float)_pendingThreadedLoads.Count / total);
                onProgress?.Invoke(progress);

                await Task.Delay(16);
            }
        }

        public T Get<T>(string name) where T : Resource
        {
            if (_registry.TryGetValue(typeof(T), out var subRegistry))
            {
                if (subRegistry.TryGetValue(name, out var resource))
                {
                    return resource as T;
                }
            }
            
            // Fallback: try base types if specific type not found
            foreach (var pair in _registry)
            {
                if (typeof(T).IsAssignableFrom(pair.Key))
                {
                    if (pair.Value.TryGetValue(name, out var resource))
                    {
                        return resource as T;
                    }
                }
            }

            Log.Err($"Resource not found: {name} (Type: {typeof(T).Name})");
            return null;
        }

        public void LoadDefinitionDirectory(string path)
        {
            DirAccess dir = DirAccess.Open(path);
            if (dir == null)
            {
                Log.Err($"Failed to open definition directory: {path}");
                return;
            }
            foreach (var file in dir.GetFiles())
            {
                FileAccess fileAccess = FileAccess.Open(path + "/" + file, FileAccess.ModeFlags.Read);
                if (fileAccess == null)
                {
                    Log.Err($"Failed to open definition file: {path}/{file}");
                    continue;
                }
                try
                {
                    DefinitionJsons.Add(path + "/" + file, fileAccess.GetAsText());
                }
                catch (Exception ex)
                {
                    Log.Err($"Failed to read definition file: {path}/{file}. Exception: {ex}");
                }
            }
        }

        public void ProcessDefinitions()
        {
            foreach (var kvp in DefinitionJsons)
            {
                Definition.Parse(kvp.Value, kvp.Key, out _);
            }
        }
    }
}
