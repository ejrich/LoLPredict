using System;
using RestSharp;

namespace RiotApi
{
    public interface IRestClientFactory
    {
        IRestClientWrapper CreateRestClient(string baseUrl);
    }

    public class RestClientFactory : IRestClientFactory
    {
        public IRestClientWrapper CreateRestClient(string name)
        {
            var baseUrl = GetEnvironmentVariable(name);
            var token = GetEnvironmentVariable("TOKEN");

            var restClient = new RestClient(baseUrl);

            return new RestClientWrapper(restClient, token);
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
