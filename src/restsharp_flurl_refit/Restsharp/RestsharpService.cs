using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using restsharp_flurl_refit.models;

namespace restsharp_flurl_refit.Restsharp
{
    public class RestsharpService
    {
        private readonly IRestClient restClient;
        private readonly string token;

        public RestsharpService(IRestClient restClient, string token)
        {
            this.token = token;
            this.restClient = restClient;
        }

        public IEnumerable<Pessoa> Get()
        {
            var response = restClient.ExecuteAsGet<List<Pessoa>>(new RestRequest("pessoa"), "GET");

            return response.Data;
        }

        public async Task<IEnumerable<Pessoa>> GetAsync()
        {
            var response = await restClient.ExecuteGetAsync<List<Pessoa>>(new RestRequest("pessoa"));

            return response.Data;
        }

        public IEnumerable<Pessoa> GetComToken()
        {
            var request = new RestRequest("pessoa");
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = restClient.ExecuteAsGet<List<Pessoa>>(request, "GET");

            return response.Data;
        }
    }
}