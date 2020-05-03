using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace RiotApi
{
    public interface IRestClientWrapper
    {
        Task<T> GetAsync<T>(string url) where T : new();
    }

    public class RestClientWrapper : IRestClientWrapper 
    {
        private readonly IRestClient _restClient;
        private readonly string _token;
        private readonly int? _rateLimit;

        private static Queue<DateTime> _apiCalls = new Queue<DateTime>();

        public RestClientWrapper(IRestClient restClient, string token, int? rateLimit)
        {
            _restClient = restClient;
            _token = token;
            _rateLimit = rateLimit;
        }

        public async Task<T> GetAsync<T>(string url) where T : new()
        {
            if (_rateLimit == null) return await MakeRequest<T>(url);

            while (_apiCalls.Any() && _apiCalls.Peek() < DateTime.Now.AddMinutes(-1)) _apiCalls.Dequeue();

            if (_apiCalls.Count < _rateLimit)
            {
                _apiCalls.Enqueue(DateTime.Now);
                return await MakeRequest<T>(url);
            }

            var earliestCall = _apiCalls.Peek();
            var waitTime = earliestCall.AddMinutes(1).Subtract(DateTime.Now);

            if (waitTime.Ticks > 0) Thread.Sleep(waitTime);

            _apiCalls.Dequeue();
            _apiCalls.Enqueue(DateTime.Now);
            return await MakeRequest<T>(url);
        }

        public async Task<T> MakeRequest<T>(string url) where T : new()
        {
            var request = BuildRequest(url);

            var response = await _restClient.GetAsync<T>(request);

            return response;
        }

        private IRestRequest BuildRequest(string url)
        {
            var request = new RestRequest(url);
            request.AddHeader("X-Riot-Token", _token);

            return request;
        }
    }
}
