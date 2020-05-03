using System;
using RestSharp;

namespace RiotApi
{
    public interface IRestClientFactory
    {
        IRestClientWrapper CreateRestClient(string baseUrl, string token);
        IRestClientWrapper CreateRestClient(string baseUrl, string token, int? rateLimit);
    }

    public class RestClientFactory : IRestClientFactory
    {
        public IRestClientWrapper CreateRestClient(string baseUrl, string token)
        {
            return CreateRestClient(baseUrl, token, null);
        }

        public IRestClientWrapper CreateRestClient(string baseUrl, string token, int? rateLimit)
        {
            var restClient = new RestClient(baseUrl);

            return new RestClientWrapper(restClient, token, rateLimit);
        }
    }
}
