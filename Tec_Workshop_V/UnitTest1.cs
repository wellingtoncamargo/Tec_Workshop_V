using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;

namespace Tec_Workshop_V
{
    public class Tests
    {
        public RestRequest endpoint;
        public RestClient client;
        public IRestResponse resp;
        public string dog_nome;
        public JArray dog_foto;

        public RestClient Client(string uri)
        {
            client = new RestClient(uri);
            return client;
        }

        public RestRequest Endpoint(string rota)
        {
            endpoint = new RestRequest(rota);
            return endpoint;
        }

        public void Get()
        {
            endpoint.Method = Method.GET;
            endpoint.RequestFormat = DataFormat.Json;
        }

        public void Post()
        {
            endpoint.Method = Method.POST;
            endpoint.RequestFormat = DataFormat.Json;
        }

        public void Params(string chave, string valor)
        {
            endpoint.AddParameter(chave, valor);
        }

        public void Body(string body)
        {
            endpoint.AddParameter(
                "application/json", 
                body, 
                ParameterType.RequestBody);
        }

        public string JsonBody()
        {
            var body = @"{
                          ""id"": 12345,
                          ""category"": {
                            ""name"": ""SRD""
                          },
                          ""name"": ""Nina Maria"",
                          ""photoUrls"": [
                            ""file:///C:/Users/wncg/Desktop/QArentena/Nina_Maria1.jpeg"",
                            ""file:///C:/Users/wncg/Desktop/QArentena/Nina_Maria2.jpeg"",
                            ""file:///C:/Users/wncg/Desktop/QArentena/Nina_Maria3.jpeg"",
                            ""file:///C:/Users/wncg/Desktop/QArentena/Nina_Maria.jpeg""
                          ],
                          ""status"": ""available""
                        }";
            return body;
        }

        public IRestResponse StatusCode(int code)
        {
            resp = client.Execute(endpoint);
            if (resp.IsSuccessful)
            {
                var status = (int)resp.StatusCode;
                Assert.AreEqual(code, status);
            }
            else
            {
                var status = (int)resp.StatusCode;
                var desc = resp.StatusDescription.ToString();
                var content = resp.Content.ToString();

                Console.WriteLine($"{status} - {desc}");
                Console.WriteLine(content);
                Assert.AreEqual(code, status);
            }
            return resp;
        }

        public void Resp()
        {
            dynamic obj = JProperty.Parse(resp.Content);
            Console.WriteLine(obj);
        }

        [Test]
        public void Buscando_Musica()
        {
            Client("https://api.vagalume.com.br");
            Endpoint("/search.php");
            Params("art", "temas-diversos");
            Params("mus", "minhoca");
            Get();
            StatusCode(200);
            Resp();


            dynamic obs = JProperty.Parse(resp.Content);
            //Console.WriteLine(obs);
            string name = obs["mus"][0]["name"];
            Assert.AreEqual("Minhoca", name);
            Console.WriteLine(name);

            string mus = obs["mus"][0]["text"];
            string V_mus = "Minhoca, minhoca me dá uma beijoca \nNão dou, não dou\nEntão eu vou roubar\n\nSmack, smack!!!\n\nMinhoco, minhoco você é mesmo louco\nVocê beijou errado, a boca é do outro lado";
            Assert.AreEqual(V_mus, mus);
            Console.WriteLine(mus);

        }
        
       
        public void Dog_novo_Pet()
        {
            Client("https://petstore.swagger.io/v2");
            Endpoint("/pet");
            Post();
            Body(JsonBody());
            StatusCode(200);
            Resp();

            dynamic obs = JProperty.Parse(resp.Content);
            dog_nome = obs["name"];
            Console.WriteLine(dog_nome);

            dog_foto = obs["photoUrls"];
            Console.WriteLine(dog_foto.ToString());
        }
        
   
        public void Adotanto_Doguinho()
        {
            Client("https://petstore.swagger.io/v2");
            Endpoint("/pet/12345");
            Get();
            StatusCode(200);
            //Resp();

            dynamic obs = JProperty.Parse(resp.Content);
            string dog_nome2 = obs["name"];
            Assert.AreEqual(dog_nome, dog_nome2);
            Console.WriteLine(dog_nome.ToString());

            JArray dog_foto2 = obs["photoUrls"];
            Assert.AreEqual(dog_foto, dog_foto2);
            Console.WriteLine(dog_foto.ToString());
        }

        [Test]
        public void Validando_Fluxo()
        {
            Dog_novo_Pet();
            Console.WriteLine("==========================================");
            Adotanto_Doguinho();
        }
    }
}