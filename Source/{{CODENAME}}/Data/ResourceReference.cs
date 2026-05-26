using Godot;
using {{CODENAME}}.Core;

namespace {{CODENAME}}.Data
{
    public class ResourceReference<[MustBeVariant] T> where T : Resource
    {
        public string Name { get; set; }

        public ResourceReference(string name)
        {
            Name = name;
        }

        public T Get()
        {
            return Game.AssetManager.Get<T>(Name);
        }
    }
}
