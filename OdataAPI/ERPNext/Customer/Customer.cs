using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OdataAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OdataAPI.ERPNext.Customer
{
    public class Customer
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _logger;

        public Customer(IHttpClientFactory clientFactory, ILogger logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IQueryable<CustomerRoot>> CustomerGet()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/resource/Customer?fields=[\"*\"]");

            var client = _clientFactory.CreateClient("erpnext");
   
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseSting = await response.Content.ReadAsStringAsync();
    
                try
                {
                     var json = JsonConvert.DeserializeObject<CustomerModel>(responseSting);
                    return json.data.AsQueryable() ;
                }
                catch (JsonReaderException ex)
                {
                   _logger.LogError("Invalid JSON."+ ex.Message);

                }

            }
            var _null = new CustomerModel { data = new CustomerRoot[] { } };
                _logger.LogWarning("No data.");

            return _null.data.AsQueryable();
        }

        public async Task<IQueryable<CustomerRoot>>GetCustomerById(string Id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/resource/Customer/"+Id+"?fields=[\"*\"]");

            var client = _clientFactory.CreateClient("erpnext");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseSting = await response.Content.ReadAsStringAsync();

                try
                {
                    var json = JsonConvert.DeserializeObject<CustomerSingleModel>(responseSting);
                    var array = new CustomerRoot[] { json.data };
                    return array.AsQueryable();
                }
                catch (JsonReaderException ex)
                {
                    _logger.LogError("Invalid JSON." + ex.Message);

                }

            }

            var _null = new CustomerRoot[] { } ;
            _logger.LogWarning("No data.");

            return _null.AsQueryable();
        }

        public async Task<CustomerRoot> CreateCystomer(CustomerRoot customer)
        {
            var ItemJson = new StringContent(JsonConvert.SerializeObject(customer),Encoding.UTF8, "application/json");
           
            var client = _clientFactory.CreateClient("erpnext");

            using var response =    await client.PostAsync("/api/resource/Customer", ItemJson);

            if (response.IsSuccessStatusCode)
            {
                var responseSting = await response.Content.ReadAsStringAsync();

                try
                {
                    var json = JsonConvert.DeserializeObject<CustomerSingleModel>(responseSting);
                    return json.data;
                }
                catch (JsonReaderException ex)
                {
                    _logger.LogError("Invalid JSON." + ex.Message);

                }

            }

            var _null = new CustomerRoot { };
            _logger.LogWarning("No data.");

            return _null;
        }

    }
}
