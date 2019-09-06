using System;

namespace LoLPredict.Web.DAL
{
    public interface IConfigurationProxy
    {
        string AzureStorageConnectionString { get; }
    }

    public class ConfigurationProxy : IConfigurationProxy
    {
        public string AzureStorageConnectionString => GetEnvironmentVariable("AZURE_STORAGE");

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
