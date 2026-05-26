using System.Threading.Tasks;

namespace {{CODENAME}}.Core
{
    public interface IInitStep
    {
        string Description { get; }
        Task Execute(InitializationContext context);
    }
}
