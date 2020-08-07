using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using restsharp_flurl_refit.models;
using restsharp_flurl_refit.Restsharp;

namespace restsharp_flurl_refit
{
    class Program
    {
        async static Task Main(string[] args)
        {

            var token = "token";
            var restClient = new RestClient(@"http://demo8717114.mockable.io/");
            var pessoa = new Pessoa {
                Nome = "Renan Aragão",
                Idade = 29,
                Sexo = 'M'
            };
            var service = new RestsharpService(restClient, token);

            Console.WriteLine("======= RestSharp =======");
            Console.WriteLine("GET /pessoa");
            Console.WriteLine(JsonConvert.SerializeObject(service.Get()));
            Console.WriteLine();
            Console.WriteLine("GET /pessoa (async)");
            Console.WriteLine(JsonConvert.SerializeObject(await service.GetAsync()));
            Console.WriteLine();
            Console.WriteLine("GET /pessoa (com token)");
            Console.WriteLine(JsonConvert.SerializeObject(service.GetComToken()));
            Console.WriteLine();
            Console.WriteLine("GET /pessoa/78");
            Console.WriteLine(JsonConvert.SerializeObject(service.Get(78)));
            Console.WriteLine();
            Console.WriteLine("GET /pessoa?id=12&nome=Renan");
            Console.WriteLine(JsonConvert.SerializeObject(service.Get(12, "Renan")));
            Console.WriteLine();
            Console.WriteLine("POST /pessoa");
            Console.WriteLine(JsonConvert.SerializeObject(service.Post(pessoa)));
        }
    }
}
