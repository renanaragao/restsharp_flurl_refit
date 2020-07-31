using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using CompareObject;
using Newtonsoft.Json;
using NSubstitute;
using RestSharp;
using restsharp_flurl_refit.models;
using restsharp_flurl_refit.Restsharp;
using Xunit;

namespace restsharp_flurl_refit.test
{
    public class RestsharpServiceTest
    {
        private readonly IRestClient restClient;
        private readonly List<Pessoa> pessoas;
        private readonly Pessoa pessoa;
        private readonly Fixture fixture;
        private readonly IRestResponse<List<Pessoa>> responsePessoas;
        private readonly IRestResponse<Pessoa> responsePessoa;
        private readonly string token;
        private readonly RestsharpService service;

        public RestsharpServiceTest()
        {
            fixture = new Fixture();
            token = fixture.Create<string>();
            pessoas = fixture.CreateMany<Pessoa>().ToList();
            pessoa = fixture.Create<Pessoa>();

            responsePessoas = new RestResponse<List<Pessoa>>
            {
                Data = pessoas
            };

            responsePessoa = new RestResponse<Pessoa>
            {
                Data = pessoa
            };

            restClient = Substitute.For<IRestClient>();
            restClient.ExecuteAsGet<List<Pessoa>>(Arg.Any<RestRequest>(), Arg.Any<string>()).Returns(responsePessoas);
            restClient.ExecuteAsGet<Pessoa>(Arg.Any<RestRequest>(), Arg.Any<string>()).Returns(responsePessoa);
            restClient.ExecuteGetAsync<List<Pessoa>>(Arg.Any<RestRequest>()).Returns(responsePessoas);
            restClient.ExecuteAsPost<Pessoa>(Arg.Any<RestRequest>(), Arg.Any<string>()).Returns(responsePessoa);

            service = new RestsharpService(restClient, token);
        }

        [Fact]
        public void Deve_Fazer_Get()
        {
            var result = service.Get();

            Assert.True(pessoas.Compare(result));

            restClient.Received().ExecuteAsGet<List<Pessoa>>(Arg.Is<RestRequest>(x => x.Resource == "pessoa"), "GET");
        }

        [Fact]
        public void Deve_Fazer_Get_Com_Bearer_Token()
        {
            var result = service.GetComToken();

            Assert.True(pessoas.Compare(result));

            restClient.Received().ExecuteAsGet<List<Pessoa>>(Arg.Is<RestRequest>(x => ValidarGetComBearerToken(x)), "GET");
        }

        private bool ValidarGetComBearerToken(RestRequest request)
        {
            if (request.Resource != "pessoa") return false;
            if (!request.Parameters.Any(x => x.Name == "Authorization" && x.Value.ToString() == $"Bearer {token}")) return false;

            return true;
        }

        [Fact]
        public async Task Deve_Fazer_Get_Async()
        {
            var result = await service.GetAsync();

            Assert.True(pessoas.Compare(result));

            await restClient.Received().ExecuteGetAsync<List<Pessoa>>(Arg.Is<RestRequest>(x => x.Resource == "pessoa"));
        }

        [Fact]
        public void Deve_Fazer_Get_Com_Query_String()
        {
            var result = service.Get(78, "Renan");

            Assert.Same(pessoa, result);

            restClient
                .Received()
                .ExecuteAsGet<Pessoa>
                (
                    Arg.Is<RestRequest>
                    (
                        x => x.Resource == "pessoa" &&
                        x.Parameters.Any(p => p.Name == "id" && p.Value.ToString() == "78") &&
                        x.Parameters.Any(p => p.Name == "nome" && p.Value.ToString() == "Renan")
                    ),
                    "GET"
                );
        }

        [Fact]
        public void Deve_Fazer_Get_Com_Parametro()
        {
            var result = service.Get(78);

            Assert.Same(pessoa, result);

            restClient
                .Received()
                .ExecuteAsGet<Pessoa>
                (
                    Arg.Is<RestRequest>
                    (
                        x => x.Resource == "pessoa/{id}" &&
                        x.Parameters.Any(p => p.Name == "id" && p.Value.ToString() == "78")
                    ),
                    "GET"
                );
        }

        [Fact]
        public void Deve_Fazer_Post()
        {
            var result = service.Post(pessoa);

            Assert.Same(pessoa, result);

            restClient
                .Received()
                .ExecuteAsPost<Pessoa>
                (
                    Arg.Is<RestRequest>
                    (
                        x => ValidarRequestPost(x)
                    ),
                    "POST"
                );

        }

        private bool ValidarRequestPost(RestRequest request)
        {
            if (request.Resource != "pessoa") return false;

            if (!request.Parameters.Any(p => p.ContentType == "application/json" && p.Value.Compare(pessoa)))
                return false;

            return true;
        }
    }
}
