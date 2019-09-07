using Microsoft.ML.Data;

namespace LoLPredict.Web.Models
{
    public class GamePrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
