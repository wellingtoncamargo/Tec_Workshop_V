using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Linq;

namespace Tec_Workshop_V
{
    public class Tests
    {
        public RestRequest endpoint;
        public RestClient client;
        public IRestResponse resp;
        public string dog_nome;
        public JArray dog_foto;
        public string dog_status;

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

        public void Put()
        {
            endpoint.Method = Method.PUT;
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
                            ""file:///C:/Users/wncg/Desktop/QArentena/Nina_Maria1.jpeg""                            
                          ],
                          ""status"": ""available""
                        }";
            return body;
        }

        public string Alterar_json(string _json, string chave, dynamic valor)
        {
            JObject Objeto_Json = JObject.Parse(_json);
            if (chave == "foto")
                Objeto_Json["photoUrls"][0] = valor;
            Objeto_Json[chave] = valor;

            string body = JsonConvert.SerializeObject(Objeto_Json);
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

        public dynamic Busca_valor(dynamic chave)
        {
            dynamic obs = JProperty.Parse(resp.Content);
            var valor = obs[chave];
            return valor;
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
        
       [Test]
        public void Dog_novo_Pet()
        {
            Client("https://petstore.swagger.io/v2");
            Endpoint("/pet");
            Post();
            Body(JsonBody());
            StatusCode(200);
            Resp();

            
            dog_nome = Busca_valor("name");
            Console.WriteLine(dog_nome);

            dog_foto = Busca_valor("photoUrls");
            Console.WriteLine(dog_foto.ToString());

            dog_status = Busca_valor("status");
            Console.WriteLine(dog_status.ToString());
        }
        
   
        public void Adotanto_Doguinho(string nome, dynamic foto, string dog_status)
        {
            Client("https://petstore.swagger.io/v2");
            Endpoint("/pet/12345");
            Get();
            StatusCode(200);
            //Resp();

            string dog_nome2 = Busca_valor("name");
            Assert.AreEqual(nome, dog_nome2);

            JArray dog_foto2 = Busca_valor("photoUrls");
            Assert.AreEqual(foto, dog_foto2);

            string dog_status2 = Busca_valor("status");
            Assert.AreEqual(dog_status, dog_status2);
        }

        public void Alterando_Informacao_Dog_Pet()
        {
            Client("https://petstore.swagger.io/v2");
            Endpoint("/pet");
            Put();
            dynamic fotos = "file:///C:/Users/wncg/Desktop/QArentena/Nina_Maria2.jpeg";
            var foto = Alterar_json(resp.Content, "foto", fotos);
            var Status = Alterar_json(foto, "status", "Adotada");
            Body(Status);
            StatusCode(200);
            Resp();

            
            dog_nome = Busca_valor("name");
            Console.WriteLine(dog_nome);

            dog_foto = Busca_valor("photoUrls");
            Console.WriteLine(dog_foto.ToString());

            dog_status = Busca_valor("status");
            Console.WriteLine(dog_foto.ToString());
        }

        [Test]
        public void Validando_Fluxo()
        {
            Dog_novo_Pet();
            Console.WriteLine("==========================================");
            Adotanto_Doguinho(dog_nome, dog_foto, dog_status);
            Console.WriteLine("==========================================");
            Alterando_Informacao_Dog_Pet();
            Console.WriteLine("==========================================");
            Adotanto_Doguinho(dog_nome, dog_foto, dog_status);
        }
    }
}