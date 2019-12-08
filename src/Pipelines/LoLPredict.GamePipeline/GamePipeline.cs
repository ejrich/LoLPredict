using System.Threading.Tasks;
using RiotApi;
using RiotApi.Models;

namespace LoLPredict.GamePipeline
{
    public interface IGamePipeline
    {
        Task LoadGames();
    }

    public class GamePipeline : IGamePipeline
    {
        private readonly IMatchGenerator _matchGenerator;

        public GamePipeline(IMatchGenerator matchGenerator)
        {
            _matchGenerator = matchGenerator;
        }

        public async Task LoadGames()
        {
            await _matchGenerator.Generate("challengerleagues");
        }
    }
}
