using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.PaymentProxy;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class PaymentProxyApiCommunicationService : IPaymentProxyApiCommunicationService
    {
        IHttpClientFactory _HttpClientFactory = null;
        ProxyPaymentApiSettings _Settings = null;
        ILogger _Logger = null;
        ITransactionQueryHistoryManager _TransactionQueryHistoryManager = null;
        IAesEncryptionService _EncryptionService = null;

        public PaymentProxyApiCommunicationService(IHttpClientFactory httpClientFactory,
            IOptions<ProxyPaymentApiSettings> options,
            ILogger<PaymentProxyApiCommunicationService> logger,
            ITransactionQueryHistoryManager transactionQueryHistoryManager,
            IAesEncryptionService aesEncryptionService)
        {
            _HttpClientFactory = httpClientFactory;
            _Settings = options.Value;
            _Logger = logger;
            _TransactionQueryHistoryManager = transactionQueryHistoryManager;
            _EncryptionService = aesEncryptionService;
        }

        public async Task<ConfirmPaymentResponse> ConfirmPayment(ConfirmPaymentRequest request, int transactionId)
        {
            var history = new TransactionQueryHistory();
            history.TransactionId = transactionId;
            history.RequestContent = _EncryptionService.Encrypt(request);
            history.CreateDate = DateTime.UtcNow;

            await _TransactionQueryHistoryManager.AddAsync(history);
            await _TransactionQueryHistoryManager.SaveAsync();

            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Settings.ApiKey);

            var url = $"{_Settings.Url}/api/payment/confirmpayment";

            var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                history.ResponseContent = content;

                history.UpdateDate = DateTime.UtcNow;
                await _TransactionQueryHistoryManager.UpdateAsync(history);
                await _TransactionQueryHistoryManager.SaveAsync();

                var result = JsonConvert.DeserializeObject<ConfirmPaymentResponse>(content);

                result.Success = true;

                return result;
            }
            else
            {
                if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    history.ResponseContent = content;

                    history.UpdateDate = DateTime.UtcNow;
                    await _TransactionQueryHistoryManager.UpdateAsync(history);
                    await _TransactionQueryHistoryManager.SaveAsync();

                    return new ConfirmPaymentResponse()
                    {
                        Success = false,
                        Message = content
                    };
                }
                _Logger.LogError($"Payment proxy api returned error for initialize payment. {response.StatusCode}");
                throw new TransactionException(TransactionResultEnum.TransactionNotConfirmed);
            }
        }

        public async Task<InitializeTransactionResponse> InitializeTransaction(InitializeTransactionRequest request, int transactionId)
        {
            var history = new TransactionQueryHistory();
            history.TransactionId = transactionId;
            history.RequestContent = JsonConvert.SerializeObject(request);
            history.CreateDate = DateTime.UtcNow;

            await _TransactionQueryHistoryManager.AddAsync(history);
            await _TransactionQueryHistoryManager.SaveAsync();

            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Settings.ApiKey);

            var url = $"{_Settings.Url}/api/payment/initpayment";

            var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                history.ResponseContent = content;

                history.UpdateDate = DateTime.UtcNow;
                await _TransactionQueryHistoryManager.UpdateAsync(history);
                await _TransactionQueryHistoryManager.SaveAsync();

                return JsonConvert.DeserializeObject<InitializeTransactionResponse>(content);
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    history.ResponseContent = content;

                    history.UpdateDate = DateTime.UtcNow;
                    await _TransactionQueryHistoryManager.UpdateAsync(history);
                    await _TransactionQueryHistoryManager.SaveAsync();
                }
                _Logger.LogError($"Payment proxy api returned error for initialize payment. {response.StatusCode}");
                throw new TransactionException(TransactionResultEnum.TransactionNotConfirmed);
            }
        }

        public async Task<SendOtpResponse> SendOtp(SendOtpRequest request, int transactionId)
        {
            var history = new TransactionQueryHistory();
            history.TransactionId = transactionId;
            history.RequestContent = JsonConvert.SerializeObject(request);
            history.CreateDate = DateTime.UtcNow;

            await _TransactionQueryHistoryManager.AddAsync(history);
            await _TransactionQueryHistoryManager.SaveAsync();

            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Settings.ApiKey);

            var url = $"{_Settings.Url}/api/payment/sendotprequest";

            var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                history.ResponseContent = content;

                history.UpdateDate = DateTime.UtcNow;
                await _TransactionQueryHistoryManager.UpdateAsync(history);
                await _TransactionQueryHistoryManager.SaveAsync();

                var otpResponse = JsonConvert.DeserializeObject<SendOtpResponse>(content);

                return otpResponse;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    history.ResponseContent = content;

                    history.UpdateDate = DateTime.UtcNow;
                    await _TransactionQueryHistoryManager.UpdateAsync(history);
                    await _TransactionQueryHistoryManager.SaveAsync();
                }
                _Logger.LogError($"Payment proxy api returned error for initialize payment. {response.StatusCode}");
                return null;
            }
        }

        public async Task<SendPaymentResponse> SendPayment(SendPaymentRequest request, int transactionId)
        {
            var history = new TransactionQueryHistory();
            history.TransactionId = transactionId;
            history.RequestContent = _EncryptionService.Encrypt(request);
            history.CreateDate = DateTime.UtcNow;

            await _TransactionQueryHistoryManager.AddAsync(history);
            await _TransactionQueryHistoryManager.SaveAsync();

            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Settings.ApiKey);

            var url = $"{_Settings.Url}/api/payment/sendpayment";

            var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                history.ResponseContent = content;

                history.UpdateDate = DateTime.UtcNow;
                await _TransactionQueryHistoryManager.UpdateAsync(history);
                await _TransactionQueryHistoryManager.SaveAsync();

                var paymentResponse = JsonConvert.DeserializeObject<SendPaymentResponse>(content);

                paymentResponse.Success = true;

                return paymentResponse;
            }
            else
            {
                string message = string.Empty;

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    history.ResponseContent = content;

                    history.UpdateDate = DateTime.UtcNow;
                    await _TransactionQueryHistoryManager.UpdateAsync(history);
                    await _TransactionQueryHistoryManager.SaveAsync();

                    var error = JsonConvert.DeserializeObject<SendPaymentErrorResponse>(content);

                    message = error.ErrorType == "Custom" ? error.Message : string.Empty;
                }
                _Logger.LogError($"Payment proxy api returned error for initialize payment. {response.StatusCode}");

                return new SendPaymentResponse()
                {
                    Success = false,
                    Message = message
                };
            }
        }
    }
}
