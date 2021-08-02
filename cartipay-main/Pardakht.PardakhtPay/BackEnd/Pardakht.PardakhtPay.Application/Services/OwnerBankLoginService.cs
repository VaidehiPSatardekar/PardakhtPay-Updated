using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;
using Pardakht.PardakhtPay.Shared.Models.WebService.Sms;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class OwnerBankLoginService : DatabaseServiceBase<OwnerBankLogin, IOwnerBankLoginManager>, IOwnerBankLoginService
    {
        IBankBotService _BankBotService = null;
        CurrentUser _CurrentUser = null;
        ICardToCardAccountManager _CardToCardAccountManager = null;
        BankBotConfiguration _BankBotConfiguration;
        //ITenantCacheService _TenantCacheService = null;
        ICardToCardAccountGroupItemManager _CardToCardGroupItemManager = null;
        ICardToCardAccountGroupManager _CardToCardGroupManager = null;
        IAesEncryptionService _EncryptionService = null;
        QRRegisterConfiguration _QRRegisterConfiguration = null;
        ISmsService _SmsService = null;
        ICachedObjectManager _CachedObjectManager = null;

        public OwnerBankLoginService(IOwnerBankLoginManager manager,
            ILogger<OwnerBankLoginService> logger,
            IBankBotService bankBotService,
            ICardToCardAccountManager cardToCardAccountManager,
            //ITenantCacheService tenantCacheService,
            IOptions<BankBotConfiguration> bankBotConfiguration,
            ICardToCardAccountGroupItemManager cardToCardAccountGroupItemManager,
            ICardToCardAccountGroupManager cardToCardAccountGroupManager,
            CurrentUser currentUser,
            IAesEncryptionService encryptionService,
            IOptions<QRRegisterConfiguration> qrRegisterOptions,
            ISmsService smsService,
            ICachedObjectManager cachedObjectManager
            ) : base(manager, logger)
        {
            _BankBotService = bankBotService;
            _CardToCardAccountManager = cardToCardAccountManager;
            _CurrentUser = currentUser;
            _BankBotConfiguration = bankBotConfiguration.Value;
            _CardToCardGroupItemManager = cardToCardAccountGroupItemManager;
            _CardToCardGroupManager = cardToCardAccountGroupManager;
            _EncryptionService = encryptionService;
            _QRRegisterConfiguration = qrRegisterOptions.Value;
            _SmsService = smsService;
            _CachedObjectManager = cachedObjectManager;
        }

        public async Task<WebResponse<BotAccountInformation>> GetAccountByGuid(string accountGuid)
        {
            try
            {
                var account = await _BankBotService.GetAccountByGuid(accountGuid);

                if (!_CurrentUser.LoginGuids.Contains(account.LoginGuid))
                {
                    throw new Exception("You don't have permission to access this account");
                }

                return new WebResponse<BotAccountInformation>(account);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<BotAccountInformation>(ex);
            }
        }

        public async Task<WebResponse<List<BotAccountInformation>>> GetAccountsAsync(bool includeDeleteds)
        {
            try
            {
                var items = await _BankBotService.GetAccountsAsync();

                items = items.Where(t => (!t.IsDeleted || includeDeleteds) && _CurrentUser.LoginGuids.Contains(t.LoginGuid)).ToList();

                return new WebResponse<List<BotAccountInformation>>(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<List<BotAccountInformation>>(ex);
            }
        }

        public async Task<WebResponse<List<BotAccountInformation>>> GetAccountsByLoginGuidAsync(string loginGuid)
        {
            try
            {
                var items = await _BankBotService.GetAccountsByLoginGuidAsync(loginGuid);

                items = items.Where(t => !t.IsDeleted && _CurrentUser.LoginGuids.Contains(t.LoginGuid)).ToList();

                return new WebResponse<List<BotAccountInformation>>(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<List<BotAccountInformation>>(ex);
            }
        }

        public async Task<WebResponse<List<BotAccountInformation>>> GetUnusedAccountsByLoginGuidAsync(string loginGuid)
        {
            try
            {
                var items = await _BankBotService.GetAccountsByLoginGuidAsync(loginGuid);

                items = items.Where(t => !t.IsDeleted && _CurrentUser.LoginGuids.Contains(t.LoginGuid)).ToList();

                var accounts = await _CardToCardAccountManager.GetUsedCardToCardAccountsAsync(loginGuid);

                items = items.Where(t => !accounts.Any(p => t.AccountGuid == p.AccountGuid)).ToList();

                return new WebResponse<List<BotAccountInformation>>(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<List<BotAccountInformation>>(ex);
            }
        }

        public async Task<WebResponse<List<BotAccountInformation>>> GetAccountsByUsernameAsync(string username)
        {
            try
            {
                var items = await _BankBotService.GetAccountsByUsernameAsync(username);

                items = items.Where(t => !t.IsDeleted && _CurrentUser.LoginGuids.Contains(t.LoginGuid)).ToList();

                return new WebResponse<List<BotAccountInformation>>(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<List<BotAccountInformation>>(ex);
            }
        }

        public async Task<WebResponse<List<BotBankInformation>>> GetBanks()
        {
            try
            {
                var banks = await _BankBotService.GetBanks();

                return new WebResponse<List<BotBankInformation>>(banks);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<List<BotBankInformation>>(ex);
            }
        }

        public async Task<WebResponse<BotLoginSelect>> GetLoginById(int id)
        {
            try
            {
                var items = await _BankBotService.GetLoginSelect();

                items = items.Where(t => !t.IsDeleted).ToList();

                var currentLogin = await Manager.GetEntityByIdAsync(id);

                var login = items.FirstOrDefault(t => t.LoginGuid == currentLogin.BankLoginGuid);

                var banks = await _BankBotService.GetBanks();

                var qrRegistrationDetails = await _BankBotService.GetQRRegistrationDetailsByLoginId(currentLogin.BankLoginId);

                var bank = banks.FirstOrDefault(t => t.Id == currentLogin.BankId);

                if (login != null)
                {
                    login.Id = currentLogin.Id;
                    login.FriendlyName = currentLogin.FriendlyName;
                    login.OwnerGuid = currentLogin.OwnerGuid;
                    login.TenantGuid = currentLogin.TenantGuid;
                    login.Status = currentLogin.Status;
                }
                else
                {
                    login = new BotLoginSelect();

                    login.FriendlyName = currentLogin.FriendlyName;
                    login.LoginGuid = currentLogin.BankLoginGuid;
                    login.Id = currentLogin.Id;
                    login.OwnerGuid = currentLogin.OwnerGuid;
                    login.Status = currentLogin.Status;
                    login.TenantGuid = currentLogin.TenantGuid;
                }
                login.LoginRequestId = currentLogin.LoginRequestId;
                login.LoginType = currentLogin.LoginType;
                login.BankId = currentLogin.BankId;
                login.IsSecondPasswordNeeded = bank.IsSecondPasswordNeeded;
                login.QRRegistrationId = qrRegistrationDetails?.Id;
                login.QRRegistrationStatus = qrRegistrationDetails?.QRRegistrationStatus;
                login.QRRegistrationStatusId = qrRegistrationDetails?.Status;
                login.BankLoginId = currentLogin.BankLoginId;

                if (!string.IsNullOrEmpty(currentLogin.AccountNumbers))
                {
                    login.Accounts = JsonConvert.DeserializeObject<List<string>>(currentLogin.AccountNumbers);
                }

                return new WebResponse<BotLoginSelect>(login);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<BotLoginSelect>(ex);
            }
        }

        public async Task<WebResponse<List<BotLoginSelect>>> GetLoginSelect(bool includeDeleteds)
        {
            try
            {
                var returnList = new List<BotLoginSelect>();

                var items = await _BankBotService.GetLoginSelect();

                items = items.Where(t => (!t.IsDeleted || includeDeleteds) && _CurrentUser.LoginGuids.Contains(t.LoginGuid)).ToList();

                var logins = await Manager.GetAllAsync();

                items.ForEach(t =>
                {
                    var login = logins.FirstOrDefault(p => t.LoginGuid == p.BankLoginGuid);

                    var select = new BotLoginSelect();

                    select.BankId = t.BankId;
                    select.BankLoginId = t.Id;
                    select.IsActive = t.IsActive;
                    select.IsBlocked = t.IsBlocked;
                    select.IsDeleted = t.IsDeleted;
                    select.LoginGuid = t.LoginGuid;
                    select.LoginRequestId = t.LoginRequestId;
                    select.LoginType = t.LoginType;
                    select.OwnerGuid = t.OwnerGuid;
                    select.Status = t.Status;
                    select.TenantGuid = t.TenantGuid;

                    if (login != null)
                    {
                        select.Id = login.Id;
                        select.FriendlyName = login.FriendlyName;
                        select.OwnerGuid = login.OwnerGuid;
                        select.TenantGuid = login.TenantGuid;
                        select.Status = login.Status;
                        select.IsActive = login.IsActive || t.IsActive;
                        select.IsDeleted = login.IsDeleted || t.IsDeleted;
                        select.LoginType = login.LoginType;
                        select.BankId = login.BankId;
                    }
                    else
                    {
                        select.IsDeleted = true;
                    }

                    returnList.Add(select);
                });

                returnList = returnList.Where(t => !t.IsDeleted || includeDeleteds).ToList();

                return new WebResponse<List<BotLoginSelect>>(returnList);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<List<BotLoginSelect>>(ex);
            }
        }

        public async Task<WebResponse<BotLoginSelect>> InsertLoginAsync(BotLoginCreateDTO login)
        {
            try
            {
                var request = new BotLoginCreateInformation()
                {
                    BankId = login.BankId,
                    Username = login.Username,
                    Password = login.Password,
                    LoginType = (int)LoginType.Card2Card,
                    CallbackUrl = _BankBotConfiguration.CallbackUrl
                };

                BotLoginInformation response = await _BankBotService.InsertLoginAsync(request);

                if (response.Id > 0)
                {
                    var item = new OwnerBankLogin();
                    item.BankLoginGuid = response.LoginGuid;
                    item.OwnerGuid = login.OwnerGuid;
                    item.TenantGuid = login.TenantGuid;
                    item.FriendlyName = login.FriendlyName;
                    item.BankLoginId = response.Id;
                    item.LoginStatus = OwnerBankLoginStatus.WaitingInformation;
                    item.BankId = login.BankId;
                    item.IsActive = true;

                    await Manager.AddAsync(item);
                    await Manager.SaveAsync();

                    await _BankBotService.ClearLoginCache();

                    var dto = new BotLoginSelect();

                    var banks = await _BankBotService.GetBanks();

                    var bank = banks.FirstOrDefault(t => t.Id == response.BankId);

                    if (bank != null)
                    {
                        dto.BankName = bank.BankName;
                    }
                    dto.FriendlyName = login.FriendlyName;
                    dto.Id = item.Id;
                    dto.LoginGuid = response.LoginGuid;
                    dto.OwnerGuid = item.OwnerGuid;
                    dto.TenantGuid = item.TenantGuid;
                    dto.BankId = item.BankId;

                    return new WebResponse<BotLoginSelect>(dto);
                }
                else
                {
                    throw new Exception("Could not create bank bot login information");
                }

            }
            catch (Exception ex)
            {
                return new WebResponse<BotLoginSelect>(ex);
            }
        }

        public async Task<WebResponse<OwnerBankLoginUpdateDTO>> UpdateAsync(OwnerBankLoginUpdateDTO item)
        {
            try
            {
                var entity = await Manager.GetEntityByIdAsync(item.Id);

                entity.FriendlyName = item.FriendlyName;

                var bankBotLogins = await _BankBotService.GetLoginSelect();
                var login = bankBotLogins.FirstOrDefault(t => t.Id == entity.BankLoginId);

                if (login != null && login.IsBlockCard != item.IsBlockCard)
                {
                    var result = await _BankBotService.ChangeIsBlockedCardFlag(new BotLoginBlockCardChangeModel()
                    {
                        IsBlockCard = item.IsBlockCard,
                        LoginId = entity.BankLoginId
                    });

                    if (!result)
                    {
                        throw new Exception("IsBlockCard flag could not be updated");
                    }

                    await _BankBotService.ClearLoginCache();
                }

                await Manager.UpdateAsync(entity);
                await Manager.SaveAsync();

                return new WebResponse<OwnerBankLoginUpdateDTO>(item);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<OwnerBankLoginUpdateDTO>(ex);
            }
        }

        public async Task<WebResponse> UpdateLoginStatus(BankBotLoginStatus request)
        {
            try
            {
                if (request.IsBlocked)
                {
                    var cardToCardAccounts = await _CardToCardAccountManager.GetUsedCardToCardAccountsAsync(request.LoginGuid);

                    for (int i = 0; i < cardToCardAccounts.Count; i++)
                    {
                        await _CardToCardGroupItemManager.ReplaceReservedAccount(cardToCardAccounts[i], CardToCardAccountGroupItemStatus.Blocked);
                    }
                }

                if (request.IsPasswordChanged)
                {
                    await ChangeLastPasswordChangeDate(request.Id, DateTime.UtcNow);
                }

                await _BankBotService.ClearLoginCache();
                await _BankBotService.ClearAccountCache();

                return new WebResponse();
            }
            catch (Exception ex)
            {
                return new WebResponse(ex);
            }
        }

        public async Task<WebResponse> UpdateLoginAsync(BankBotUpdateLoginDTO login)
        {
            try
            {
                var entity = await Manager.GetEntityByIdAsync(login.Id);
                var logins = await _BankBotService.GetLogins();

                var bankLogin = logins.FirstOrDefault(t => t.LoginGuid == entity.BankLoginGuid);

                var response = await _BankBotService.UpdateLoginAsync(new BankBotLoginUpdateInformation()
                {
                    BankId = bankLogin.BankId,
                    Id = bankLogin.Id,
                    Password = login.Password,
                    Username = login.Username,
                    MobilePassword = login.MobilePassword,
                    MobileUsername = login.MobileUsername,
                    LoginType = bankLogin.LoginType,
                    LoginGuid = bankLogin.LoginGuid,
                    CallbackUrl = bankLogin.CallbackUrl,
                    SecondPassword = login.SecondPassword,
                    MobileNumber = login.MobileNumber,
                    EmailAddress = login.EmailAddress,
                    EmailPassword = login.EmailPassword,
                    ProcessCountIn24Hrs =Convert.ToInt32(login.ProcessCountIn24Hrs)
                });

                return new WebResponse();

            }
            catch (Exception ex)
            {
                return new WebResponse(ex);
            }
        }

        public async Task<WebResponse<BotLoginSelect>> CreateLoginRequest(BotLoginCreateDTO login)
        {
            try
            {
                var callbackUrl = _CurrentUser.CallbackUrl;

                var request = new BotLoginCreateInformation()
                {
                    BankId = login.BankId,
                    Username = login.Username,
                    Password = login.Password,
                    MobileUsername = login.MobileUsername,
                    MobilePassword = login.MobilePassword,
                    LoginType = (int)LoginType.Card2Card,
                    CallbackUrl = callbackUrl ?? _BankBotConfiguration.CallbackUrl,
                    IsMobileLogin = login.IsMobileLogin
                };

                BotLoginInformation loginRequestResponse = await _BankBotService.CreateLoginRequest(request);

                if (loginRequestResponse.Id > 0)
                {
                    var item = new OwnerBankLogin();
                    item.BankLoginGuid = string.Empty;
                    item.OwnerGuid = login.OwnerGuid;
                    item.TenantGuid = login.TenantGuid;
                    item.FriendlyName = login.FriendlyName;
                    item.BankLoginId = 0;
                    item.LoginRequestId = loginRequestResponse.Id;
                    if (loginRequestResponse.BankId == (int)BankNames.Mellat)
                    {
                        item.Status = 5;
                    }
                    else { item.LoginStatus = OwnerBankLoginStatus.WaitingInformation; }
                    item.BankId = login.BankId;
                    item.IsActive = true;

                    await Manager.AddAsync(item);
                    await Manager.SaveAsync();

                    var dto = new BotLoginSelect();

                    var banks = await _BankBotService.GetBanks();


                    dto.FriendlyName = login.FriendlyName;
                    dto.Id = item.Id;
                    dto.OwnerGuid = item.OwnerGuid;
                    dto.TenantGuid = item.TenantGuid;
                    dto.BankId = item.BankId;

                    return new WebResponse<BotLoginSelect>(dto);
                }
                else
                {
                    throw new Exception("Could not create bank bot login information");
                }
            }
            catch (Exception ex)
            {
                return new WebResponse<BotLoginSelect>(ex);
            }
        }

        public async Task<WebResponse> LoginRequestChanged(BankBotLoginRequestChangedDTO model)
        {
            try
            {
                var login = Manager.GetLoginWithLoginRequestId(model.Id);

                if (login != null)
                {
                    if (model.Success)
                    {
                        login.LoginStatus = OwnerBankLoginStatus.WaitingApprovement;

                        login.AccountNumbers = JsonConvert.SerializeObject(model.Accounts);
                    }
                    else
                    {
                        login.LoginStatus = OwnerBankLoginStatus.Error;
                    }

                    await Manager.UpdateAsync(login);
                    await Manager.SaveAsync();

                    return new WebResponse();
                }
                else
                {
                    throw new Exception($"Login could not be found with login request id {model.Id}");
                }
            }
            catch (Exception ex)
            {
                return new WebResponse(ex);
            }
        }
        public async Task<WebResponse<BotLoginInformation>> CreateLoginFromLoginRequest(CreateLoginFromLoginRequestDTO model)
        {
            try
            {
                if (!_CurrentUser.HasRole(Permissions.SelectLoginType) || !Enum.IsDefined(typeof(LoginType), model.LoginType))
                {
                    model.LoginType = (int)LoginType.Card2Card;
                }

                var response = await _BankBotService.CreateLoginFromLoginRequest(model);

                var login = Manager.GetLoginWithLoginRequestId(model.LoginRequestId);
                login.LoginStatus = OwnerBankLoginStatus.Success;
                login.BankLoginId = response.Id;
                login.BankLoginGuid = response.LoginGuid;

                await Manager.UpdateAsync(login);
                await Manager.SaveAsync();

                await _BankBotService.ClearLoginCache();
                await _BankBotService.ClearAccountCache();

                return new WebResponse<BotLoginInformation>(response);
            }
            catch (Exception ex)
            {
                return new WebResponse<BotLoginInformation>(ex);
            }
        }

        public async Task<WebResponse<List<BotLoginSelect>>> GetOwnerLoginList()
        {
            try
            {
                var selects = await _BankBotService.GetLoginSelect();

                var logins = await Manager.GetAllAsync();

                var items = new List<BotLoginSelect>();

                logins.ForEach(login =>
                {
                    if (!login.IsDeleted)
                    {
                        var item = new BotLoginSelect();
                        item.FriendlyName = login.FriendlyName;
                        item.LoginGuid = login.BankLoginGuid;
                        item.Id = login.Id;
                        item.OwnerGuid = login.OwnerGuid;
                        item.Status = login.Status;
                        item.TenantGuid = login.TenantGuid;
                        item.IsDeleted = login.IsDeleted;
                        item.IsActive = login.IsActive;
                        item.LoginType = login.LoginType;
                        item.BankId = login.BankId;
                        item.LastPasswordChangeDate = login.LastPasswordChangeDate;
                        
                        var select = selects.FirstOrDefault(t => t.LoginGuid == login.BankLoginGuid);

                        if (select != null)
                        {
                            item.BankLoginId = select.Id;
                            item.BankId = select.BankId;
                            item.BankName = select.BankName;
                            item.IsBlocked = select.IsBlocked;
                            item.IsDeleted = item.IsDeleted || select.IsDeleted;
                            item.BankConnectionProgram = select.BankConnectionProgram;
                        }

                        items.Add(item);
                    }
                });

                return new WebResponse<List<BotLoginSelect>>(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<List<BotLoginSelect>>(ex);
            }
        }

        public async Task<WebResponse<BotLoginInformation>> DeactivateLogin(int id)
        {
            try
            {
                var login = await Manager.GetEntityByIdAsync(id);

                if (login == null)
                {
                    throw new Exception("Login is not found");
                }

                var bankLogins = await _BankBotService.GetLogins();

                var bankLogin = bankLogins.FirstOrDefault(t => t.LoginGuid == login.BankLoginGuid);

                if (bankLogin == null)
                {
                    throw new Exception("Login information could not be found on the bank bot");
                }

                login.IsActive = false;
                await Manager.UpdateAsync(login);

                var item = await _BankBotService.DeactivateLoginByUsername(bankLogin.Username);

                await Manager.SaveAsync();

                await _BankBotService.ClearAccountCache();
                await _BankBotService.ClearLoginCache();

                return new WebResponse<BotLoginInformation>(item);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<BotLoginInformation>(ex);
            }
        }

        public async Task<WebResponse<BotLoginInformation>> ActivateLogin(int id)
        {
            try
            {
                var login = await Manager.GetEntityByIdAsync(id);

                if (login == null)
                {
                    throw new Exception("Login is not found");
                }

                var bankLogins = await _BankBotService.GetLogins();

                var bankLogin = bankLogins.FirstOrDefault(t => t.LoginGuid == login.BankLoginGuid);

                if (bankLogin == null)
                {
                    throw new Exception("Login information could not be found on the bank bot");
                }

                login.IsActive = true;
                await Manager.UpdateAsync(login);

                var item = await _BankBotService.ActivateLoginByUsername(bankLogin.Username);

                await Manager.SaveAsync();

                await _BankBotService.ClearAccountCache();
                await _BankBotService.ClearLoginCache();

                return new WebResponse<BotLoginInformation>(item);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<BotLoginInformation>(ex);
            }
        }

        public async Task<WebResponse> DeleteLogin(int id)
        {
            try
            {
                var login = await Manager.GetEntityByIdAsync(id);

                if (login == null)
                {
                    throw new Exception("Login is not found");
                }

                login.IsDeleted = true;
                await Manager.UpdateAsync(login);

                if (login.BankLoginId != 0)
                {
                    await _BankBotService.DeleteLogin(login.BankLoginId);

                    await _BankBotService.ClearAccountCache();
                    await _BankBotService.ClearLoginCache();
                }

                await Manager.SaveAsync();

                return new WebResponse()
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse(ex);
            }
        }

        public async Task<WebResponse> AccountStatusChanged(AccountStatusChangedDTO model)
        {
            try
            {
                if (!Helper.IsAccountStatusTextOpen(model.Status))
                {
                    var cardToCardAccounts = await _CardToCardAccountManager.GetCardToCardAccounts(model.Guid);

                    for (int i = 0; i < cardToCardAccounts.Count; i++)
                    {
                        await _CardToCardGroupItemManager.ReplaceReservedAccount(cardToCardAccounts[i], CardToCardAccountGroupItemStatus.Blocked);
                    }
                }

                await _BankBotService.ClearLoginCache();
                await _BankBotService.ClearAccountCache();

                return new WebResponse();
            }
            catch (Exception ex)
            {
                return new WebResponse(ex);
            }
        }

        public async Task<WebResponse<List<BotBlockedCardDetail>>> GetBlockedCardDetails(string accountGuid)
        {
            try
            {
                List<BotBlockedCardDetail> items = null;

                if (accountGuid == "all")
                {
                    items = await _BankBotService.GetBlockedCardDetails();
                }
                else
                {
                    items = await _BankBotService.GetBlockedCardDetails(accountGuid);
                }

                var accounts = await _BankBotService.GetAccountsAsync();

                var logins = _CurrentUser.LoginGuids;

                accounts = accounts.Where(t => logins.Contains(t.LoginGuid)).ToList();

                items = items.Where(t => accounts.Any(p => t.AccountId == p.Id)).ToList();

                items = items.OrderByDescending(t => t.TimeStamp).ToList();

                return new WebResponse<List<BotBlockedCardDetail>>(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<List<BotBlockedCardDetail>>(ex);
            }
        }

        public async Task<WebResponse<string>> GetPassword(int loginId)
        {
            try
            {
                var login = await Manager.GetEntityByIdAsync(loginId);

                if (login == null || !_CurrentUser.LoginGuids.Contains(login.BankLoginGuid))
                {
                    throw new NullReferenceException($"Could not get login by login id {loginId}");
                }

                var bankLogin = await _BankBotService.GetLoginByIdFromServer(login.BankLoginId);

                if (bankLogin == null)
                {
                    throw new NullReferenceException($"Could not get bank login by login id {login.BankLoginId}");
                }

                return new WebResponse<string>(bankLogin.Password);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<string>(ex);
            }
        }

        public async Task<WebResponse<LoginCardNumberDto>> GetCardNumber(int loginId)
        {
            try
            {
                var logins = await _BankBotService.GetLogins();

                var bankLogin = logins.FirstOrDefault(t => t.Id == loginId);

                if (bankLogin == null)
                {
                    throw new Exception($"Login could not be found with login id : {loginId}");
                }

                var account = await _CardToCardAccountManager.GetItemAsync(t => t.LoginGuid == bankLogin.LoginGuid);

                if (account == null)
                {
                    throw new Exception($"Account could not be found with login guid : {bankLogin.LoginGuid}");
                }

                var cardNumber = _EncryptionService.DecryptToString(account.CardNumber);

                return new WebResponse<LoginCardNumberDto>()
                {
                    Success = true,
                    Payload = new LoginCardNumberDto()
                    {
                        CardNumber = cardNumber,
                        LoginId = loginId
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<LoginCardNumberDto>(ex);
            }
        }

        private async Task<OwnerBankLogin> ChangeLastPasswordChangeDate(int id, DateTime? date)
        {
            var item = await Manager.GetItemAsync(t => t.BankLoginId == id);

            item.LastPasswordChangeDate = date;

            await Manager.UpdateAsync(item);
            await Manager.SaveAsync();

            return item;
        }

        public async Task<WebResponse<BotLoginSelect>> CreateQRRegisterRequest(int loginId)
        {
            try
            {

                var callbackUrl = _CurrentUser.CallbackUrl;

                var request = new QRRegisterLoginDTO()
                {
                    LoginId = loginId
                };

                var response = await _BankBotService.CreateQRRegisterRequest(loginId);

                if (response != null)
                {
                    return new WebResponse<BotLoginSelect>(response);

                }
                else
                {
                    throw new Exception("Could not create bank bot login information");
                }
            }
            catch (Exception ex)
            {
                return new WebResponse<BotLoginSelect>(ex);
            }
        }

        public async Task<WebResponse<QRRegisterLoginDTO>> GetQRRegistrationDetailsByLoginId(int loginId)
        {
            try
            {
                var qrRegistrationDetails = await _BankBotService.GetQRRegistrationDetailsByLoginId(loginId);

                return new WebResponse<QRRegisterLoginDTO>(qrRegistrationDetails);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<QRRegisterLoginDTO>(ex);
            }
        }

        public async Task<WebResponse<QRRegisterLoginDTO>> RegisterQRCode(QrCodeRegistrationRequest qrRegisterDetails)
        {
            try
            {

                var callbackUrl = _CurrentUser.CallbackUrl;

                var response = await _BankBotService.RegisterQRCode(qrRegisterDetails);

                if (response != null)
                {
                    return new WebResponse<QRRegisterLoginDTO>(response);
                }
                else
                {
                    throw new Exception("Could not create bank bot login information");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<QRRegisterLoginDTO>(ex);
            }
        }

        public async Task<WebResponse<string>> GetLoginDeviceStatusByLoginId(string loginId)
        {
            try
            {
                var login = await Manager.GetEntityByIdAsync(Convert.ToInt32(loginId));

                if (login == null || !_CurrentUser.LoginGuids.Contains(login.BankLoginGuid))
                {
                    throw new NullReferenceException($"Could not get login by login id {loginId}");
                }

                var items = await _BankBotService.GetLoginSelect();

                var bankLogin = items.Where(t => !t.IsDeleted && t.Id == login.BankLoginId && t.MobileNumber != null).FirstOrDefault();

                if (bankLogin == null)
                {
                    throw new NullReferenceException($"Could not get bank login or no mobile number associated with the login.");
                }

                try { await SendSmsViaService(new SmsPhoneNumberModel { InvoiceKey = "", PhoneNumber = bankLogin.MobileNumber }, bankLogin.LoginGuid); }
                catch (Exception ex)
                {
                    throw new NullReferenceException($"Error while sending SMS.");
                }
                var result = await _BankBotService.GetLoginDeviceStatusByLoginId(login.BankLoginId);

                if (result == null)
                {
                    throw new NullReferenceException($"Could not get bank login by login id {login.BankLoginId}");
                }

                return new WebResponse<string>(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<string>(ex);
            }
        }

        public async Task<WebResponse<BotLoginInformation>> GetDeviceStatusOfLogins()
        {
            try
            {
                var items = await _BankBotService.GetLoginSelect();

                items = items.Where(t => !t.IsDeleted && t.MobileNumber != null).ToList();

                if (items.Count == 0)
                {
                    throw new NullReferenceException($"Could not get logins");
                }

                List<int> bankLoginIds = new List<int>();
                foreach (BotLoginSelect login in items)
                {
                    try { await SendSmsViaService(new SmsPhoneNumberModel { InvoiceKey = "", PhoneNumber = login.MobileNumber }, login.LoginGuid); }
                    catch (Exception ex)
                    {
                        throw new NullReferenceException($"Error while sending SMS.");
                    }
                    bankLoginIds.Add(login.BankLoginId);
                }

                var bankLogin = await _BankBotService.GetDeviceStatusOfLogins(bankLoginIds);

                if (bankLogin == null)
                {
                    throw new NullReferenceException($"Could not get bank login device status");
                }

                return new WebResponse<BotLoginInformation>(bankLogin);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<BotLoginInformation>(ex);
            }
        }


        public async Task<WebResponse<List<BotLoginSelect>>> GetOwnerLoginListWithDeviceStatus()
        {
            try
            {
                var selects = await _BankBotService.GetLoginSelect();

                var logins = await Manager.GetAllAsync();

                var items = new List<BotLoginSelect>();

                logins.ForEach(login =>
                {
                    if (!login.IsDeleted)
                    {
                        var item = new BotLoginSelect();
                        item.FriendlyName = login.FriendlyName;
                        item.LoginGuid = login.BankLoginGuid;
                        item.Id = login.Id;
                        item.OwnerGuid = login.OwnerGuid;
                        item.Status = login.Status;
                        item.TenantGuid = login.TenantGuid;
                        item.IsDeleted = login.IsDeleted;
                        item.IsActive = login.IsActive;
                        item.LoginType = login.LoginType;
                        item.BankId = login.BankId;
                        item.LastPasswordChangeDate = login.LastPasswordChangeDate;

                        var select = selects.FirstOrDefault(t => t.LoginGuid == login.BankLoginGuid);

                        if (select != null)
                        {
                            item.BankLoginId = select.Id;
                            item.BankId = select.BankId;
                            item.BankName = select.BankName;
                            item.IsBlocked = select.IsBlocked;
                            item.IsDeleted = item.IsDeleted || select.IsDeleted;
                            item.LoginDeviceStatusId = select.LoginDeviceStatusId;
                        }

                        items.Add(item);
                    }
                });

                return new WebResponse<List<BotLoginSelect>>(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<List<BotLoginSelect>>(ex);
            }
        }

        private async Task SendSmsViaService(SmsPhoneNumberModel model, string bankLoginGuild)
        {
            var response = await _SmsService.SendSms(new SmsServiceRequest()
            {
                Message = bankLoginGuild,
                PhoneNumber = model.PhoneNumber,
                SecretKey = string.Empty,
                TemplateId = string.Empty,
                UserApiKey = string.Empty
            });

            if (!response.IsSent)
            {
                throw new Exception("Sms could not be sended");
            }

        }

        public async Task<WebResponse<string>> GenerateOTPForRegisteredDevice(int id)
        {
            try
            {
                var currentLogin = await Manager.GetEntityByIdAsync(id);

                var callbackUrl = _CurrentUser.CallbackUrl;

                var response = await _BankBotService.GenerateOTPForRegisteredDevice(currentLogin.BankLoginId);

                if (response != null)
                {
                    return new WebResponse<string>(response);
                }
                else
                {
                    throw new Exception("Could not create bank bot login information");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<string>(ex);
            }
        }

        public async Task<WebResponse<BotLoginSelect>> RegisterLoginRequest(RegisterLogin login)
        {
            try
            {

                var callbackUrl = _CurrentUser.CallbackUrl;

                var request = new RegisterLogin()
                {
                    BankId = login.BankId,
                    LoginRequestId = login.LoginRequestId,
                    Otp = login.Otp
                };

                BotLoginInformation registerLoginResponse = await _BankBotService.RegisterLoginRequest(request);

                if (registerLoginResponse != null)
                {
                    var item = new OwnerBankLogin();
                    item.BankLoginGuid = string.Empty;
                    item.BankLoginId = 0;
                    item.LoginRequestId = login.LoginRequestId;
                    item.LoginStatus = OwnerBankLoginStatus.WaitingInformation;
                    item.BankId = registerLoginResponse.BankId;
                    item.IsActive = true;

                    await Manager.AddAsync(item);
                    await Manager.SaveAsync();

                    var dto = new BotLoginSelect();

                    var banks = await _BankBotService.GetBanks();


                    //  dto.FriendlyName = registerLoginResponse.FriendlyName;
                    dto.Id = item.Id;
                    dto.OwnerGuid = item.OwnerGuid;
                    dto.TenantGuid = item.TenantGuid;
                    dto.BankId = item.BankId;

                    return new WebResponse<BotLoginSelect>(dto);
                }
                else
                {
                    throw new Exception("Could not create bank bot login information");
                }
            }
            catch (Exception ex)
            {
                return new WebResponse<BotLoginSelect>(ex);
            }
        }

        public async Task<WebResponse<string>> SwitchBankConnectionProgram(int loginId)
        {
            try
            {
                var login = await Manager.GetEntityByIdAsync(loginId);

                if (login == null || !_CurrentUser.LoginGuids.Contains(login.BankLoginGuid))
                {
                    throw new NullReferenceException($"Could not get login by login id {loginId}");
                }

                var bankLogin = await _BankBotService.SwitchBankConnectionProgram(login.BankLoginId);

                if (bankLogin == null)
                {
                    throw new NullReferenceException($"Could not get bank login by login id {login.BankLoginId}");
                }

                return new WebResponse<string>(bankLogin.BankConnectionProgram);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<string>(ex);
            }
        }

        public async Task ClearCache()
        {
            await _CachedObjectManager.ClearCachedItems<CardToCardAccountGroupItem>();
            await _CachedObjectManager.ClearCachedItems<CardToCardAccountGroup>();
            await _CachedObjectManager.ClearCachedItems<CardToCardUserSegmentRelation>();
        }

    }
}
