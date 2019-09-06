using LoLPredict.Database.Models;

namespace LoLPredict.Pipelines.DAL
{
    public static class ModelExtensions
    {
        public static string Number(this Patch patch)
        {
            return $"{patch.Major}.{patch.Minor}.{patch.Version}";
        } 
    }
}
