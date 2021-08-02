using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IOwnerBankLoginService : IServiceBase<OwnerBankLogin, IOwnerBankLoginManager>
    {
        Task<WebResponse<BotLoginSelect>> InsertLoginAsync(BotLoginCreateDTO login);

        Task<WebResponse> UpdateLoginAsync(BankBotUpdateLoginDTO login);

        Task<WebResponse<List<BotLoginSelect>>> GetLoginSelect(bool includeDeleteds);

        Task<WebResponse<List<BotLoginSelect>>> GetOwnerLoginList();

        Task<WebResponse<List<BotAccountInformation>>> GetAccountsByUsernameAsync(string username);

        Task<WebResponse<BotAccountInformation>> GetAccountByGuid(string accountGuid);

        Task<WebResponse<List<BotAccountInformation>>> GetAccountsAsync(bool includeDeleteds);

        Task<WebResponse<List<BotAccountInformation>>> GetAccountsByLoginGuidAsync(string loginGuid);

        Task<WebResponse<List<BotBankInformation>>> GetBanks();

        Task<WebResponse<BotLoginSelect>> GetLoginById(int id);

        Task<WebResponse<OwnerBankLoginUpdateDTO>> UpdateAsync(OwnerBankLoginUpdateDTO item);

        Task<WebResponse<List<BotAccountInformation>>> GetUnusedAccountsByLoginGuidAsync(string loginGuid);

        Task<WebResponse> UpdateLoginStatus(BankBotLoginStatus request);

        Task<WebResponse<BotLoginSelect>> CreateLoginRequest(BotLoginCreateDTO login);

        Task<WebResponse> LoginRequestChanged(BankBotLoginRequestChangedDTO model);

        Task<WebResponse<BotLoginInformation>> CreateLoginFromLoginRequest(CreateLoginFromLoginRequestDTO model);

        Task<WebResponse<BotLoginInformation>> DeactivateLogin(int id);

        Task<WebResponse<BotLoginInformation>> ActivateLogin(int id);

        Task<WebResponse> DeleteLogin(int id);

        Task<WebResponse> AccountStatusChanged(AccountStatusChangedDTO model);

        Task<WebResponse<List<BotBlockedCardDetail>>> GetBlockedCardDetails(string accountGuid);

        Task<WebResponse<string>> GetPassword(int loginId);

        Task<WebResponse<LoginCardNumberDto>> GetCardNumber(int loginId);

        Task<WebResponse<BotLoginSelect>> CreateQRRegisterRequest(int loginId);

        Task<WebResponse<QRRegisterLoginDTO>> GetQRRegistrationDetailsByLoginId(int loginId);

        Task<WebResponse<QRRegisterLoginDTO>> RegisterQRCode(QrCodeRegistrationRequest qrRegisterDetails);
        Task<WebResponse<string>> GetLoginDeviceStatusByLoginId(string loginId);

        Task<WebResponse<BotLoginInformation>> GetDeviceStatusOfLogins();

        Task<WebResponse<List<BotLoginSelect>>> GetOwnerLoginListWithDeviceStatus();

        Task<WebResponse<string>> GenerateOTPForRegisteredDevice(int bankLoginId);

        Task<WebResponse<BotLoginSelect>> RegisterLoginRequest(RegisterLogin login);

        Task<WebResponse<string>> SwitchBankConnectionProgram(int loginId);        

        Task ClearCache();
    }
}
