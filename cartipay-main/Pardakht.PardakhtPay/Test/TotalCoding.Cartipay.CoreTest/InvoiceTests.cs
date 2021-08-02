using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TotalCoding.Cartipay.CoreTest
{
    [TestClass]
    public class InvoiceTests
    {
        [TestMethod]
        private void GetTokenTest()
        {
            string domainAddress = "http://localhost:53724";
            string orderId = "4434";
            string apiKey = "1700c70a650704b0e456bb3f81fe6a90c12a902f6e7f8b6e123d5fffe196";
            string amount = "5000";
            string customerGuid = "ddddddddd";

            string invoiceKey = string.Empty;
            string returnUrl = "http://test";

            var client = new RestClient(domainAddress + "/invoice/request");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("content-type", $"multipart/form-data; boundary=----WebKitFormBoundary{orderId}");
            request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary{orderId}", $"------WebKitFormBoundary{orderId}\r\nContent-Disposition: form-data; name=\"api_key\"\r\n\r\n{apiKey}\r\n------WebKitFormBoundary{orderId}\r\nContent-Disposition: form-data; name=\"return_url\"\r\n\r\n{returnUrl}\r\n------WebKitFormBoundary{orderId}\r\nContent-Disposition: form-data; name=\"amount\"\r\n\r\n{amount}\r\n------WebKitFormBoundary{orderId}\r\nContent-Disposition: form-data; name=\"userid\"\r\n\r\n{customerGuid}\r\n------WebKitFormBoundary{orderId}--", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                var obj = JObject.Parse(response.Content);
                var status = obj["status"].ToString();
                invoiceKey = obj["invoice_key"].ToString();
            }

            Assert.IsTrue(string.IsNullOrEmpty(invoiceKey));
        }
    }
}
