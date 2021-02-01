using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OdataAPI;
using OdataAPI.ERPNext.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class CustomerTests
    {
        private readonly HttpClient client;

        public CustomerTests()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>();
            var server = new TestServer(webHostBuilder);
            client = server.CreateClient();
        }

        [TestMethod]
        public async Task MetadataTest()
        {
            // Act
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/odata/$metadata");

            HttpResponseMessage response = await client.SendAsync(request);

            Assert.IsTrue(response.IsSuccessStatusCode, $"Status code: " + response.StatusCode.ToString());
        }

        [TestMethod]
        public async Task BatchTestAsync()
        {
            HttpRequestMessage batchRequest = new HttpRequestMessage(HttpMethod.Post, "/odata/$batch");
            
            MultipartContent multipartContent = new MultipartContent("mixed", "batch_" + Guid.NewGuid().ToString());

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/odata/Customer?$orderby=Name&$select=Name&$skip=0&$top=20");
            var messageContent = new HttpMessageContent(requestMessage);

            if (messageContent.Headers.Contains("Content-Type"))
            {
                messageContent.Headers.Remove("Content-Type");
            }
            messageContent.Headers.Add("Content-Type", "application/http");
            messageContent.Headers.Add("Content-Transfer-Encoding", "binary");
            requestMessage.Headers.Add("Accept", "application/json");

            multipartContent.Add(messageContent);

            requestMessage.Content = multipartContent;

            HttpResponseMessage response = await client.SendAsync(requestMessage);

            var responseString = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseString);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Status code: " + response.StatusCode.ToString());
        }

        [DataTestMethod]
        [DataRow("CUST0000001")]
        [DataRow("CUST0000002")]
        [DataRow("CUST0000003")]
        [DataRow("CUST0000004")]
        public async Task Customer_Single_Ok_Result(string key)
        {
            bool result = false;

            // Act
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/odata/Customer(" + key + ")");

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(responseString))
                {
                    var _result = JsonConvert.DeserializeObject<CustomerModel>(responseString);
                    if (_result != null)
                    {
                        result = true;
                    }
                    else { result = false; }
                }
                else { result = false; }
            }
            else
            {
                result = false;
            }
            // Assert
            Assert.IsTrue(result, $"TRUE iD: {key} status code:" + response.StatusCode);
        }

        [DataTestMethod]
        [DataRow("KLI1")]
        [DataRow("C0000001")]
        [DataRow("3")]
        [DataRow("1")]
        public async Task Customer_Single_False_Result(string key)
        {
            bool result = false;

            // Act
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/odata/Customer(" + key + ")");

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(responseString))
                {
                    var _result = JsonConvert.DeserializeObject<CustomerRoot>(responseString);
                    if (_result != null)
                    {
                        result = true;
                    }
                    else { result = false; }
                }
                else { result = false; }
            }
            else
            {
                result = false;
            }
            // Assert
            Assert.IsFalse(result, $"False iD: {key} status code:" + response.StatusCode);
        }

        [TestMethod]
        public async Task CustomerListTest()
        {
            bool _result = false;
            int count = 0;
            // Act
            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Get, "/odata/Customer");

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                var objResult = JsonConvert.DeserializeObject<IQueryable<CustomerRoot>>(responseString);

                count = objResult.Count();
                _result = true;
            }
            else
            {
                _result = false;
            }

            Assert.IsTrue(_result, $"Customer test - [ " + count + " ] rows");
        }
    }
}
