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

        public RestClientWrapper(IRestClient restClient, string token)
        {
            _restClient = restClient;
            _token = token;
        }

        public async Task<T> GetAsync<T>(string url) where T : new()
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
