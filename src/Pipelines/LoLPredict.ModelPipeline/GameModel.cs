using Microsoft.ML;

namespace LoLPredict.ModelPipeline
{
    public class GameModel
    {
        public string Patch { get; set; }
        public ITransformer Model { get; set; }
    }
}
