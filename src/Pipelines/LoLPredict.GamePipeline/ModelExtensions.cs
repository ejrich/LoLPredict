using LoLPredict.Database.Models;

namespace LoLPredict.GamePipeline
{
    public static class ModelExtensions
    {
        public static bool AnyUnassignedRoles(this GameResult game)
        {
            return game.BlueTop == 0 ||
                   game.BlueJungle == 0 ||
                   game.BlueMid == 0 ||
                   game.BlueBottom == 0 ||
                   game.BlueSupport == 0 ||
                   game.RedTop == 0 ||
                   game.RedJungle == 0 ||
                   game.RedMid == 0 ||
                   game.RedBottom == 0 ||
                   game.RedSupport == 0;
        }
    }
}
