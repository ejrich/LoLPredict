using System.Threading.Tasks;

namespace LoLPredict.ModelPipeline
{
    public interface IModelFactory
    {
        Task<GameModel> CreateModel();
    }
}
