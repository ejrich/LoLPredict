using System.Linq;
using LoLPredict.Database.Models;
using LoLPredict.ModelPipeline.Domain.Models;
using LoLPredict.Pipelines.DAL;
using Microsoft.ML;
using Microsoft.ML.Transforms;

namespace LoLPredict.ModelPipeline.ChampionModel
{
    public class ChampionModelPipeline : IModelFactory
    {
        private readonly MLContext _context;
        private readonly IGameRepository _gameRepository;

        private static readonly string[] Positions = {
            "BlueTop", "BlueJungle", "BlueMid", "BlueBottom", "BlueSupport", "RedTop", "RedJungle", "RedMid", "RedBottom", "RedSupport"
        };

        public ChampionModelPipeline(MLContext context, IGameRepository gameRepository)
        {
            _context = context;
            _gameRepository = gameRepository;
        }

        public GameModel CreateModel()
        {
            var livePatch = _gameRepository.LoadLivePatch();
            if (livePatch == null)
                return null;

            var championIds = _gameRepository.LoadChampions(livePatch.Number())
                .Select(_ => new ChampionId { Id = _.Id });

            var uniqueChampions = _context.Data.LoadFromEnumerable(championIds);

            var gameResults = _gameRepository.LoadGameResults(livePatch.Number())
                .Select(MapGameToInputGame);

            var data = _context.Data.LoadFromEnumerable(gameResults);

            var pipeline = CreateOneHotEncoding("BlueTop", uniqueChampions)
                .Append(CreateOneHotEncoding("BlueJungle", uniqueChampions))
                .Append(CreateOneHotEncoding("BlueMid", uniqueChampions))
                .Append(CreateOneHotEncoding("BlueBottom", uniqueChampions))
                .Append(CreateOneHotEncoding("BlueSupport", uniqueChampions))
                .Append(CreateOneHotEncoding("RedTop", uniqueChampions))
                .Append(CreateOneHotEncoding("RedJungle", uniqueChampions))
                .Append(CreateOneHotEncoding("RedMid", uniqueChampions))
                .Append(CreateOneHotEncoding("RedBottom", uniqueChampions))
                .Append(CreateOneHotEncoding("RedSupport", uniqueChampions))
                .Append(_context.Transforms.Concatenate("Features", Positions))
                .AppendCacheCheckpoint(_context)
                .Append(_context.BinaryClassification.Trainers.SdcaLogisticRegression());

            var model = pipeline.Fit(data);

            return new GameModel
            {
                Patch = livePatch.Number(),
                Model = model
            };
        }

        private OneHotEncodingEstimator CreateOneHotEncoding(string columnName, IDataView keyData)
        {
            return _context.Transforms.Categorical.OneHotEncoding(columnName, keyData: keyData);
        }

        private static InputGame MapGameToInputGame(GameResult game)
        {
            return new InputGame
            {
                BlueTop = game.BlueTop,
                BlueJungle = game.BlueJungle,
                BlueMid = game.BlueMid,
                BlueBottom = game.BlueBottom,
                BlueSupport = game.BlueSupport,
                RedTop = game.RedTop,
                RedJungle = game.RedJungle,
                RedMid = game.RedMid,
                RedBottom = game.RedBottom,
                RedSupport = game.RedSupport,
                WinningTeam = game.Winner
            };
        }
    }
}
