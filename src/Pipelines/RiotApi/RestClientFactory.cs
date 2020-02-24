using System;
using RestSharp;

namespace RiotApi
{
    public interface IRestClientFactory
    {
        IRestClientWrapper CreateRestClient(string baseUrl, string token);
    }

    public class RestClientFactory : IRestClientFactory
    {
        public IRestClientWrapper CreateRestClient(string baseUrl, string token)
        {
            var restClient = new RestClient(baseUrl);

            return new RestClientWrapper(restClient, token);
        }
    }
}
