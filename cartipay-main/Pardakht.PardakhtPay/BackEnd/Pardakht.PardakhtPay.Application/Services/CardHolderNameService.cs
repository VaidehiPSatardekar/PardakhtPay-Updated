using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class CardHolderNameService : DatabaseServiceBase<CardHolderName, ICardHolderNameManager>, ICardHolderNameService
    {
        IAesEncryptionService _EncryptionService = null;
        static string _Token = string.Empty;
        IHttpClientFactory _HttpClientFactory = null;
        UserManagementSettings _UserManagementSettings = null;

        public CardHolderNameService(ICardHolderNameManager manager,
            ILogger<CardHolderNameService> logger,
            IAesEncryptionService aesEncryptionService,
            IHttpClientFactory httpClientFactory,
            IOptions<UserManagementSettings> userManagementOptions):base(manager, logger)
        {
            _EncryptionService = aesEncryptionService;
            _HttpClientFactory = httpClientFactory;
            _UserManagementSettings = userManagementOptions.Value;
        }

        public async Task<WebResponse<CardHolderNameDTO>> GetCardHolderName(string cardNumber)
        {
            try
            {

                if (!cardNumber.CheckCardNumberIsValid())
                {
                    throw new Exception($"Card number is invalid {cardNumber}");
                }

                var item = await Manager.GetCardHolderName(cardNumber, string.Empty, string.Empty);

                return new WebResponse<CardHolderNameDTO>(new CardHolderNameDTO()
                {
                    Id = item.Id,
                    CardHolderName = item.Name,
                    CardNumber = _EncryptionService.DecryptToString(item.CardNumber).GetBeautyCardNumber(),
                    Type = item.Type,
                    Status= item.Status
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<CardHolderNameDTO>(ex);
            }
        }

        public async Task<WebResponse<CardHolderNameDTO>> GetIbanName(string iban)
        {
            try
            {

                if (!iban.CheckIbanIsValid())
                {
                    throw new Exception($"Iban number is invalid {iban}");
                }

                var item = await Manager.GetIbanName(iban, string.Empty, string.Empty);

                return new WebResponse<CardHolderNameDTO>(new CardHolderNameDTO()
                {
                    Id = item.Id,
                    CardHolderName = item.Name,
                    CardNumber = _EncryptionService.DecryptToString(item.CardNumber).GetBeautyCardNumber(),
                    Type = item.Type,
                    Status=item.Status
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<CardHolderNameDTO>(ex);
            }
        }

        public async Task<WebResponse<CardHolderNameDTO>> GetAccountName(CardHolderNameDTO model)
        {
            try
            {
                CardHolderName item = null;
                if (model.Type == (int)CardHolderNameType.Iban)
                {
                    if (!model.CardNumber.CheckIbanIsValid())
                    {
                        throw new Exception($"Iban number is invalid {model.CardNumber}");
                    }

                    item = await Manager.GetIbanName(model.CardNumber, model.CallbackUrl, model.AccountContext);
                }
                else if(model.Type == (int)CardHolderNameType.CardNumber)
                {
                    if (!model.CardNumber.CheckCardNumberIsValid())
                    {
                        throw new Exception($"Card number is invalid {model.CardNumber}");
                    }

                    item = await Manager.GetCardHolderName(model.CardNumber, model.CallbackUrl, model.AccountContext);
                }

                return new WebResponse<CardHolderNameDTO>(new CardHolderNameDTO()
                {
                    IsSuccess = item.Status != (int)CardHolderRequestStatus.Failed,
                    Id = item.Id,
                    CardHolderName = item.Name,
                    CardNumber = _EncryptionService.DecryptToString(item.CardNumber),
                    Type = item.Type,
                    Status=item.Status
                    
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<CardHolderNameDTO>(ex);
            }
        }

        public async Task<WebResponse> UpdateCardHolderName(CardHolderNameResponse response)
        {
            try
            {
                var oldItem = await Manager.GetItemAsync(t => t.RecordId == response.Id);

                if(oldItem == null)
                {
                    var encrypted = _EncryptionService.EncryptToBase64(response.CardORIBANNumber);

                    oldItem = await Manager.GetItemAsync(t => t.CardNumber == response.CardORIBANNumber && t.Type == response.Type);
                }

                if (oldItem != null)
                {
                    oldItem.UpdateDate = DateTime.UtcNow;
                    oldItem.Name = response.Name;
                    oldItem.Status = (int)response.Status;

                    await Manager.UpdateAsync(oldItem);
                    await Manager.SaveAsync();
                }
                else
                {
                    oldItem = new CardHolderName();
                    oldItem.CardNumber = _EncryptionService.EncryptToBase64(response.CardORIBANNumber);
                    oldItem.CreateDate = DateTime.UtcNow;
                    oldItem.Name = response.Name;
                    oldItem.RecordId = response.Id;
                    oldItem.UpdateDate = DateTime.UtcNow;
                    oldItem.Type = response.Type;
                    oldItem.Status = response.Status;

                    await Manager.AddAsync(oldItem);
                    await Manager.SaveAsync();
                }

                if (!string.IsNullOrEmpty(oldItem.CallbackUrl))
                {
                    await InformCaller(oldItem);
                }

                return new WebResponse();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse(ex);
            }
        }

        private async Task InformCaller(CardHolderName oldItem)
        {
            try
            {
                if (string.IsNullOrEmpty(_Token))
                {
                    await GetToken();
                }

                HttpResponseMessage response = await CallApi(oldItem);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await GetToken();

                    await CallApi(oldItem);
                }

                if (response.IsSuccessStatusCode)
                {
                    Logger.LogInformation($"Informing caller for card holder name received successful response : { oldItem.Id }");
                }
                else
                {
                    Logger.LogInformation($"Informing caller for card holder name received unsuccessful response : { oldItem.Id } {(int)response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Informing caller for card holder name received an error : { oldItem.Id } {ex.Message}");
            }
        }

        private async Task<HttpResponseMessage> CallApi(CardHolderName oldItem)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _Token);

            var item = JsonConvert.SerializeObject(new CardHolderNameDTO
            {
                IsSuccess = oldItem.Status == (int)CardHolderRequestStatus.Complete,
                Id = oldItem.Id,
                CardNumber = _EncryptionService.DecryptToString(oldItem.CardNumber),
                Type = oldItem.Type,
                CardHolderName = oldItem.Name,
                AccountContext = oldItem.AccountContext,
                Status= oldItem.Status
                
            });

            if (!string.IsNullOrEmpty(oldItem.AccountContext))
            {
                client.DefaultRequestHeaders.Add("account-context", oldItem.AccountContext);
            }

            var response = await client.PostAsync(oldItem.CallbackUrl, new StringContent(item, Encoding.UTF8, "application/json"));
            return response;
        }

        private async Task GetToken()
        {
            var client = _HttpClientFactory.CreateClient();

            string url = _UserManagementSettings.Url + "/api/StaffUser/api-key-login";

            using (var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new
            {
                apiKey = _UserManagementSettings.ApiKey,
                platformGuid = _UserManagementSettings.PlatformGuid
            }), Encoding.UTF8, "application/json")))
            {

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);

                    _Token = tokenResponse.accessToken;
                }
            }
        }
    }
}
