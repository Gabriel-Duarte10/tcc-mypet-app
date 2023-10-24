using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcc_mypet_app.Services
{ 
    public class ViaCEPService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<Address> GetAddressByCEPAsync(string cep)
        {
            string url = $"https://viacep.com.br/ws/{cep}/json/";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                var fullAddress = JsonConvert.DeserializeObject<FullAddress>(result);
                if (fullAddress.UF == null)
                {
                    throw new Exception("Cep não existe");
                }
                return new Address
                {
                    Street = fullAddress.Logradouro,
                    City = fullAddress.Localidade,
                    UF = fullAddress.UF
                };
            }
            else
            {
                throw new Exception("Cep não existe");
            }
        }
        public class Address
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string UF { get; set; }
        }
        public class FullAddress
        {
            public string CEP { get; set; }
            public string Logradouro { get; set; }
            public string Complemento { get; set; }
            public string Bairro { get; set; }
            public string Localidade { get; set; }
            public string UF { get; set; }
            public string Unidade { get; set; }
            public string IBGE { get; set; }
            public string GIA { get; set; }
        }
    }
}
