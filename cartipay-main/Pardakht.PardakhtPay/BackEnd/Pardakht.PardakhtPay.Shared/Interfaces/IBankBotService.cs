using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    /// <summary>
    /// Represents and interface to check a payment's status from bank bot
    /// </summary>
    public interface IBankBotService
    {
        /// <summary>
        /// Confirmes payment from bank bot
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BotConfirmationResponse> Confirm(BotConfirmRequest request);

        /// <summary>
        /// Confirms payment from bank bot
        /// </summary>
        /// <param name="token">The token of the transaction</param>
        /// <returns></returns>
        Task<BotConfirmationResponse> Confirm(string token);

        Task<BotWithdrawalResponse> Transfer(BotWithdrawalRequest request);

        Task<BotTransferStatusResponse> CheckTransferStatus(int id);

        Task<List<BotBankInformation>> GetBanks();

        Task<List<BotLoginInformation>> GetLogins();

        Task<List<BotLoginSelect>> GetLoginSelect();

        Task<BotLoginInformation> InsertLoginAsync(BotLoginCreateInformation login);

        Task<BotLoginInformation> UpdateLoginAsync(BankBotLoginUpdateInformation login);

        Task<List<BotAccountInformation>> GetAccountsByUsernameAsync(string username);

        Task<BotAccountInformation> GetAccountByGuid(string accountGuid);

        Task<List<BotAccountInformation>> GetAccountsAsync();

        Task<List<BankBotAccountWithStatusDTO>> GetAccountsWithStatus();
        Task<List<BankBotAccountWithStatusDTO>> GetAccountsWithStatuses(string[] accountGuids);

        Task<List<BankBotAccountWithStatusDTO>> GetSingleAccountsWithStatus(string accountGuid, TransferType? transferType = null);

        Task<BankBotAccountStatus> GetAccountStatus(string accountGuid);

        Task<List<BotAccountInformation>> GetAccountsByLoginGuidAsync(string loginGuid);

        Task<BotAccountInformation> InsertAccountAsync(BankBotAccountCreateDTO item);

        Task<BotLoginInformation> CreateLoginRequest(BotLoginCreateInformation login);

        Task<BotLoginInformation> CreateLoginFromLoginRequest(CreateLoginFromLoginRequestDTO model);

        Task<BankBotTransferRequest> CreateTransferRequest(BotTransferRequestDTO model);

        Task<BankBotTransferRequest> GetTransferRequestWithStatus(int id);

        Task<BankBotTransferRequest> DeleteTransferRequest(int id);

        Task<BankBotTransferRequest> CancelTransferRequest(int id);

        Task<BankBotTransferRequest> CancelTransferRequest(string requestGuid);

        Task<BankBotTransferRequest> RetryTransferRequest(int id);

        Task<BankBotTransferRequest> RetryTransferRequest(string requestGuid);

        Task<BankBotTransferRequest> SetAsCompletedTransferRequest(int id);

        Task<BankBotTransferRequest> SetAsCompletedTransferRequest(string requestGuid);

        Task<BotLoginInformation> DeactivateLoginByUsername(string username);

        Task<BotLoginInformation> ActivateLoginByUsername(string username);

        Task<BotTransferHistoryResponseDTO> GetTransferHistory(int requestId);

        Task<BankBotTransferReceiptResponse> GetTransferReceipt(BankBotTransferReceiptRequest request);

        Task<bool> SendMobileTransferInformation(MobileTransactionDTO model);

        Task<bool> ChangeIsBlockedCardFlag(BotLoginBlockCardChangeModel model);

        Task<List<BotBlockedCardDetail>> GetBlockedCardDetails();

        Task<List<BotBlockedCardDetail>> GetBlockedCardDetails(string accountGuid);

        Task<BotCardHolderRequest> CreateCardHolderRequest(BotCardHolderRequest request);

        Task<List<BotTransferStatus>> GetTransferStatusList();

        Task DeleteLogin(int id);

        Task ClearLoginCache();

        Task ClearAccountCache();

        int AccountReadCount { get; }

        int BankReadCount { get; }

        int LoginReadCount { get; }

        Task<bool> IsSatnaApplicable(int bankId);

        Task<BotLoginInformation> GetLoginByIdFromServer(int id);

        Task<BotLoginSelect> CreateQRRegisterRequest(int loginId);

        Task<QRRegisterLoginDTO> GetQRRegistrationDetailsByLoginId(int loginId);

        Task<QRRegisterLoginDTO> RegisterQRCode(QrCodeRegistrationRequest qrRegisterDetails);

        Task<string> GetLoginDeviceStatusByLoginId(int loginId);

        Task<BotLoginInformation> GetDeviceStatusOfLogins(List<int> loginIds);

        Task<string> GenerateOTPForRegisteredDevice(int bankLoginId);

        Task<BotLoginInformation> RegisterLoginRequest(RegisterLogin login);

        Task<List<BankBotAccountWithStatusDTO>> GetMultipleAccountsWithStatus(string accountGuid, TransferType? transferType = null);

        Task<BotLoginInformation> SwitchBankConnectionProgram(int id);
    }
}
