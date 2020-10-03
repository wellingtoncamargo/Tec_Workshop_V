using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;

namespace Tec_Workshop_V
{
    public class Tests
    {
        public RestClient baseurl;
        public RestRequest rota;

        public RestClient BaseUrl(string uri)
        {
            baseurl = new RestClient(uri);
            return baseurl;
        }
        public RestRequest Rota(string endpoint)
        {
            rota = new RestRequest(endpoint);
            return rota;
        }

        public void Get()
        {
            rota.Method = Method.GET;
            rota.RequestFormat = DataFormat.Json;
        }

        public void Params(string chave, string valor)
        {
            rota.AddParameter(chave, valor, ParameterType.QueryString);
        }

        public IRestResponse Response()
        {
            IRestResponse resp = baseurl.Execute(rota);
            return resp;
        }

        [Test]
        public void ProcurandoMusica()
        {
            BaseUrl("https://api.vagalume.com.br");
            Rota("/search.php");

            Params("art", "temas-diversos");
            Params("mus", "minhoca");
            Get();

            var resp = Response();
            dynamic obs = JProperty.Parse(resp.Content);
            Console.WriteLine(obs.ToString());
        }
    }
}