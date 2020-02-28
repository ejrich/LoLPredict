namespace LoLPredict.Pipelines.DAL
{
    public class Settings
    {
        public string AzureStorageConnectionString { get; set; }
        public string StaticDataEndpoint { get; set; }
        public string RiotApiUrl { get; set; }
        public string RiotApiToken { get; set; }
    }
}
