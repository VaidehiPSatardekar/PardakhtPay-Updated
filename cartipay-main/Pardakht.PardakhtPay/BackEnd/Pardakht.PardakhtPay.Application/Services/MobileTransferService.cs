using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.MobileTransfer;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class MobileTransferService : IMobileTransferService
    {
        MobileTransferConfiguration _Configuration = null;
        IHttpClientFactory _HttpClientFactory = null;

        public MobileTransferService(IOptions<MobileTransferConfiguration> mobileTransferOptions,
            IHttpClientFactory httpClientFactory)
        {
            _Configuration = mobileTransferOptions.Value;
            _HttpClientFactory = httpClientFactory;
        }

        public async Task<MobileTransferResponse> ActivateDeviceAsync(MobileTransferActivateDeviceModel model)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(model.MobileNo), "MobileNo");
                formData.Add(new StringContent(model.VerificationCode), "VerificationCode");
                formData.Add(new StringContent(model.ApiType.ToString()), "ApiType");

                return await Send<MobileTransferResponse>("api/device/activate", formData);
            }
        }

        public async Task<MobileTransferResponse> CheckDeviceStatusAsync(MobileTransferSendSmsModel model)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(model.MobileNo), "MobileNo");
                formData.Add(new StringContent(model.ApiType.ToString()), "ApiType");

                return await Send<MobileTransferResponse>("api/device/checkisregistered", formData);
            }
        }

        public async Task<MobileTransferResponse> CheckStatusAsync(MobileTransferCheckStatusModel model)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(model.MobileNo), "MobileNo");
                formData.Add(new StringContent(model.ApiType.ToString()), "ApiType");

                return await Send<MobileTransferResponse>("api/device/CheckStatus", formData);
            }
        }

        public async Task<MobileTransferResponse> CheckTransferStatusAsync(MobileTransferStartTransferModel model)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(model.TransactionToken), "ExternalId");
                formData.Add(new StringContent(model.UniqueId ?? ""), "UniqueId");
                formData.Add(new StringContent(model.MobileNo), "MobileNo");
                formData.Add(new StringContent(model.ApiType.ToString()), "ApiType");
                formData.Add(new StringContent(model.FromCardNo), "FromCardNo");
                formData.Add(new StringContent(model.ToCardNo), "ToCardNo");

                return await Send<MobileTransferResponse>("api/Transfer/CheckStatus", formData);
            }
        }

        public async Task<MobileTransferResponse> RemoveDeviceAsync(MobileTransferRemoveDeviceModel model)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(model.MobileNo), "MobileNo");
                formData.Add(new StringContent(model.ApiType.ToString()), "ApiType");

                return await Send<MobileTransferResponse>("api/device/Remove", formData);
            }
        }

        public async Task<MobileTransferResponse> SendOtpPinAsync(MobileTransferStartTransferModel model)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(model.MobileNo), "MobileNo");
                formData.Add(new StringContent(model.FromCardNo), "FromCardNo");
                formData.Add(new StringContent(model.Amount.ToString()), "Amount");
                formData.Add(new StringContent(model.ApiType.ToString()), "ApiType");
                formData.Add(new StringContent(model.ToCardNo), "ToCardNo");

                return await Send<MobileTransferResponse>("api/transfer/sendotppin", formData);
            }
        }

        public async Task<MobileTransferResponse> SendSMSAsync(MobileTransferSendSmsModel model)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(model.MobileNo), "MobileNo");
                formData.Add(new StringContent(model.ApiType.ToString()), "ApiType");

                return await Send<MobileTransferResponse>("api/device/SendSMS", formData);
            }
        }

        public async Task<MobileTransferResponse> StartTransferAsync(MobileTransferStartTransferModel model)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(model.MobileNo), "MobileNo");
                formData.Add(new StringContent(model.FromCardNo), "FromCardNo");
                formData.Add(new StringContent(model.ToCardNo), "ToCardNo");
                formData.Add(new StringContent(model.Cvv2), "CVV2");
                formData.Add(new StringContent(model.CardPin), "CardPIN");
                formData.Add(new StringContent(model.ExpiryMonth), "ExpiryMonth");
                formData.Add(new StringContent(model.ExpiryYear), "ExpiryYear");
                formData.Add(new StringContent(model.Amount.ToString()), "Amount");
                formData.Add(new StringContent(model.TransactionToken), "ExternalId");
                formData.Add(new StringContent(model.ApiType.ToString()), "ApiType");

                return await Send<MobileTransferResponse>("api/Transfer/Start", formData);
            }
        }

        public async Task<MobileTransferResponse> GetCardOwnerNameAsync(MobileTransferStartTransferModel model)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(model.ToCardNo), "ToCardNo");
                formData.Add(new StringContent(model.ApiType.ToString()), "ApiType");

                return await Send<MobileTransferResponse>("api/Transfer/GetCardHolderName", formData);
            }
        }

        private async Task<T> Send<T>(string uri, HttpContent httpContent)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            using (var response = await client.PostAsync($"{_Configuration.ServiceUrl}/{uri}", httpContent))
            {
                if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception($"Mobile Transfer Api return unsuccessfull result for ({uri}). {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(content);
            }
        }
    }
}
