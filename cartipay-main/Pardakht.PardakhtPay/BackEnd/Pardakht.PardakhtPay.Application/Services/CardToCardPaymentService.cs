using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class CardToCardPaymentService : BasePaymentTransactionService, ICardToCardPaymentService
    {
        IBankBotService _BankBotService = null;
        IUserSegmentGroupManager _UserSegmentGroupManager = null;
        ICardToCardAccountGroupItemManager _MerchantCardToCardAccountRelationManager;
        IBankStatementItemManager _BankStatementItemManger = null;
        ITransactionRequestContentRepository _TransactionRequestContentRepository = null;

        public CardToCardPaymentService(ILogger<CardToCardPaymentService> logger,
            IAesEncryptionService encryptionService,
            ICachedObjectManager cachedObjectManager,
            ITransactionManager manager,
            IMerchantManager merchantManager,
            IMerchantCustomerManager merchantCustomerManager,
            IBankBotService bankBotService,
            IUserSegmentGroupManager userSegmentGroupManager,
            IApplicationSettingService applicationSettingService,
            ICardToCardAccountGroupItemManager relationManager,
            ITransactionQueueService queueService,
            IBankStatementItemManager bankStatementItemManager,
            IServiceProvider provider,
            ITransactionRequestContentRepository transactionRequestContentRepository) : base(logger, encryptionService, cachedObjectManager, manager, merchantManager, merchantCustomerManager, queueService, provider, applicationSettingService)
        {
            _BankBotService = bankBotService;
            _UserSegmentGroupManager = userSegmentGroupManager;
            _MerchantCardToCardAccountRelationManager = relationManager;
            _BankStatementItemManger = bankStatementItemManager;
            _TransactionRequestContentRepository = transactionRequestContentRepository;
        }

        /// <summary>
        /// Creates transaction to db and returns token of transaction
        /// </summary>
        /// <param name="request">Request information about transaction</param>
        /// <param name="ipAddress">Requested ip address</param>
        /// <returns></returns>
        public override async Task<TransactionResult<Transaction>> CreateTransaction(CreateTransactionRequest request, string ipAddress)
        {
            try
            {
                var merchant = await CheckTransactionCreating(request);

                if (!string.IsNullOrEmpty(request.Reference))
                {
                    var tran = await Manager.GetTransactionByReference(merchant.Id, request.Reference);

                    if (tran != null)
                    {
                        return new TransactionResult<Transaction>(tran);
                    }
                }

                if (!string.IsNullOrEmpty(request.CustomerCardNumber))
                {
                    if (!request.CustomerCardNumber.CheckCardNumberIsValid())
                    {
                        throw new TransactionException(TransactionResultEnum.InvalidCardNumber);
                    }
                }

                MerchantCustomerAccount customerAccount = null;
                CardToCardAccount account = null;
                UserSegmentGroup group = null;

                if (!string.IsNullOrEmpty(request.UserId) && !string.IsNullOrEmpty(request.WebsiteName))
                {
                    var customer = new MerchantCustomer();
                    customer.ActivityScore = request.UserActivityScore;
                    customer.GroupName = request.UserGroupName;
                    customer.LastActivity = request.UserLastActivity;
                    customer.MerchantId = merchant.Id;
                    customer.OwnerGuid = merchant.OwnerGuid;
                    customer.RegisterDate = request.UserRegisterDate;
                    customer.TenantGuid = merchant.TenantGuid;
                    customer.TotalDeposit = request.UserTotalDeposit;
                    customer.TotalWithdraw = request.UserTotalWithdraw;
                    customer.UserId = request.UserId;
                    customer.WebsiteName = request.WebsiteName;
                    customer.DepositNumber = request.UserDepositNumber;
                    customer.WithdrawNumber = request.UserWithdrawNumber;
                    customer.UserSportbookNumber = request.UserSportbookNumber;
                    customer.UserTotalSportbook = request.UserTotalSportbook;
                    customer.UserCasinoNumber = request.UserCasinoNumber;
                    customer.UserTotalCasino = request.UserTotalCasino;

                    customer = await MerchantCustomerManager.AddOrUpdateCustomer(customer);

                    customerAccount = await MerchantCustomerManager.GetCardToCardTransferAccounts(customer, merchant.Id);
                    if (customerAccount != null)
                    {
                        account = customerAccount.Account;
                    }
                }
                else
                {
                    group = await _UserSegmentGroupManager.GetGroup(merchant.OwnerGuid, (Dictionary<int, object>)null);

                    customerAccount = await MerchantCustomerManager.GetCardToCardAccount(merchant.Id, null, true, null, group);

                    if (customerAccount != null)
                    {
                        account = customerAccount.Account;
                    }
                }

                string accountNo = string.Empty;
                string cardNumber = string.Empty;
                string cardHolderName = string.Empty;
                string accountGuid = string.Empty;
                string loginGuid = string.Empty;
                int cardToCardAccountId = 0;

                Transaction transaction = new Transaction();

                if (customerAccount != null && customerAccount.Customer != null)
                {
                    await CheckPhoneNumber(transaction, customerAccount.Customer.ConfirmedPhoneNumber);
                }

                if ((customerAccount != null && customerAccount.UserSegmentGroup != null && customerAccount.UserSegmentGroup.IsMalicious) || transaction.IsPhoneNumberBlocked)
                {
                    var configuration = await ApplicationSettingService.Get<MaliciousCustomerSettings>();

                    cardNumber = EncryptionService.EncryptToBase64(configuration.FakeCardNumber);
                    cardHolderName = EncryptionService.EncryptToBase64(configuration.FakeCardHolderName);

                    accountGuid = string.Empty;
                    loginGuid = string.Empty;
                }
                else
                {
                    if (account == null || !account.IsActive)
                    {
                        throw new TransactionException(TransactionResultEnum.AccountNotFound);
                    }

                    var bankBotAccount = await _BankBotService.GetAccountByGuid(account.AccountGuid);

                    if (bankBotAccount.IsDeleted)
                    {
                        throw new TransactionException(TransactionResultEnum.AccountNotFound);
                    }

                    accountNo = bankBotAccount.AccountNo;

                    cardNumber = account.CardNumber;
                    cardHolderName = account.CardHolderName;
                    cardToCardAccountId = account.Id;
                    accountGuid = account.AccountGuid;
                    loginGuid = account.LoginGuid;
                }

                var token = Guid.NewGuid().ToString();

                transaction.CreationDate = DateTime.UtcNow;
                transaction.CustomerCardNumber = null;
                transaction.IpAddress = ipAddress ?? string.Empty;
                transaction.MerchantId = merchant.Id;
                transaction.ReturnUrl = request.ReturnUrl;
                transaction.Token = token;
                transaction.TransactionAmount = request.Amount;
                transaction.TransactionStatus = TransactionStatus.Started;
                transaction.TransferredDate = null;
                transaction.UpdatedDate = null;
                transaction.CardNumber = cardNumber;
                transaction.AccountNumber = EncryptionService.EncryptToBase64(accountNo);
                transaction.CardHolderName = cardHolderName;
                transaction.CardToCardAccountId = cardToCardAccountId;
                transaction.TenantGuid = merchant.TenantGuid;
                transaction.OwnerGuid = merchant.OwnerGuid;
                transaction.AccountGuid = accountGuid;
                transaction.LoginGuid = loginGuid;
                //transaction.Description = request.Description;
                transaction.RequestContent = EncryptionService.Encrypt(request);
                transaction.MerchantCustomerId = customerAccount?.Customer?.Id;
                transaction.Reference = request.Reference;
                transaction.HideCardNumber = customerAccount == null || customerAccount.GroupItem == null ? false : customerAccount.GroupItem.HideCardNumber;
                transaction.PaymentType = (int)PaymentType.CardToCard;

                if (!string.IsNullOrEmpty(request.CustomerCardNumber))
                {
                    transaction.CustomerCardNumber = EncryptionService.EncryptToBase64(request.CustomerCardNumber);
                }

                if (customerAccount != null && customerAccount.UserSegmentGroup != null)
                {
                    transaction.IsMaliciousCustomer = customerAccount.UserSegmentGroup.IsMalicious;
                    transaction.UserSegmentGroupId = customerAccount.UserSegmentGroup.Id;
                }
                else if (group != null)
                {
                    transaction.IsMaliciousCustomer = group.IsMalicious;
                    transaction.UserSegmentGroupId = group.Id;
                }

                await Manager.AddAsync(transaction);
                await Manager.SaveAsync();

                await _TransactionRequestContentRepository.InsertAsync(new TransactionRequestContent()
                {
                    TransactionId = transaction.Id,
                    RequestContent = transaction.RequestContent
                });

                await _TransactionRequestContentRepository.SaveChangesAsync();

                return new TransactionResult<Transaction>(transaction);
            }
            catch (TransactionException ex)
            {
                Logger.LogError(ex, ex.Message);
                return ex.CreateResult<Transaction>();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new TransactionResult<Transaction>(TransactionResultEnum.UnknownError);
            }
        }

        /// <summary>
        /// Returns payment information(Tenant card number, amount etc.) related with the transaction.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<TransactionResult<TransactionPaymentInformation>> GetPaymentInformation(PaymentInformationRequest request)
        {
            try
            {
                TransactionPaymentInformationWithTransaction response = await CheckPaymentInformationConditions(request);

                bool empty = false;

                if (!response.Transaction.IsMaliciousCustomer && !response.Transaction.IsPhoneNumberBlocked)
                {
                    var account = await _BankBotService.GetAccountStatus(response.Transaction.AccountGuid);

                    var accounts = await CachedObjectManager.GetCachedItems<CardToCardAccount, ICardToCardAccountRepository>();

                    if (response.Transaction.CardToCardAccountId.HasValue)
                    {
                        var acc = accounts.FirstOrDefault(t => t.Id == response.Transaction.CardToCardAccountId);

                        var logins = await _BankBotService.GetLoginSelect();

                        var login = logins.FirstOrDefault(t => t.LoginGuid == response.Transaction.LoginGuid);

                        empty = !acc.IsActive || !account.IsOpen || login.IsBlocked || login.IsDeleted || response.Transaction.HideCardNumber;
                    }
                }

                var info = new TransactionPaymentInformation()
                {
                    Amount = response.Amount,
                    CardNumber = empty ? string.Empty : response.CardNumber,
                    CardHolderName = empty ? string.Empty : response.CardHolderName,
                    ReturnUrl = response.Transaction.ReturnUrl,
                    TransactionId = response.Transaction.Id,
                    CreationDate = response.Transaction.CreationDate,
                    PaymentType = PaymentType.CardToCard,
                    Status = response.Transaction.Status
                };

                if (!string.IsNullOrEmpty(response.Transaction.CustomerCardNumber))
                {
                    info.CustomerCardNumber = EncryptionService.DecryptToString(response.Transaction.CustomerCardNumber);
                }

                return new TransactionResult<TransactionPaymentInformation>(response.Result, info);
            }
            catch (TransactionException ex)
            {
                Logger.LogError(ex, ex.Message);
                return ex.CreateResult<TransactionPaymentInformation>();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new TransactionResult<TransactionPaymentInformation>(TransactionResultEnum.UnknownError);
            }
        }

        /// <summary>
        /// Completes payment related with a transaction. After saving to database, it checks payment status from bank bot.
        /// If payment status is unsuccessful, it is added to the queue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<CompletePaymentResponse> CompletePayment(CompletePaymentRequest request)
        {
            try
            {
                var result = await Manager.CheckTransactionReadyForComplete(request.Token);

                var transaction = result.Item;

                if (!string.IsNullOrEmpty(transaction.CustomerCardNumber))
                {
                    request.CustomerCardNumber = EncryptionService.DecryptToString(transaction.CustomerCardNumber);
                }

                request.CustomerCardNumber = request.CustomerCardNumber.Trim();

                if (!request.CustomerCardNumber.CheckCardNumberIsValid())
                {
                    transaction.TransactionStatus = TransactionStatus.Expired;
                    transaction.ExternalMessage = "Invalid Card Number";

                    await Manager.UpdateAsync(transaction);
                    await Manager.SaveAsync();

                    throw new TransactionException(TransactionResultEnum.InvalidCardNumber);
                }

                var response = new CompletePaymentResponse(result.Result);

                response.Amount = Convert.ToInt32(transaction.TransactionAmount);
                response.BankNumber = string.Empty;
                response.ReturnUrl = transaction.ReturnUrl;
                response.Token = transaction.Token;
                response.PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile;

                transaction.IpAddress = request.IpAddress;

                if (!string.IsNullOrEmpty(request.CustomerCardNumber))
                {
                    response.CardNumber = request.CustomerCardNumber;//.MaskCardNumber();
                }

                if (result.Result == TransactionResultEnum.Success)
                {
                    var merchantCardNumber = EncryptionService.DecryptToString(transaction.CardNumber);

                    if (request.CustomerCardNumber == merchantCardNumber)
                    {
                        response.ResultCode = TransactionResultEnum.SameCardNumber;
                        transaction.CustomerCardNumber = EncryptionService.EncryptToBase64(request.CustomerCardNumber);
                        transaction.TransactionStatus = TransactionStatus.Fraud;

                        await Manager.UpdateAsync(transaction);
                        await Manager.SaveAsync();

                        return response;
                    }

                    transaction.CustomerCardNumber = EncryptionService.EncryptToBase64(request.CustomerCardNumber);
                    transaction.UpdatedDate = DateTime.UtcNow;
                    transaction.TransactionStatus = TransactionStatus.WaitingConfirmation;

                    await Manager.UpdateAsync(transaction);
                    await Manager.SaveAsync();

                    if (!transaction.IsMaliciousCustomer && !transaction.IsPhoneNumberBlocked)
                    {
                        var merchant = await MerchantManager.GetEntityByIdAsync(transaction.MerchantId);

                        var accounts = await _MerchantCardToCardAccountRelationManager.GetAllRelations(merchant.Id, true, null);

                        List<BankBotConfirmAccount> accountList = new List<BankBotConfirmAccount>();
                        var cardToCardAccounts = await CachedObjectManager.GetCachedItems<CardToCardAccount, ICardToCardAccountRepository>();

                        for (int i = 0; i < accounts.Count; i++)
                        {
                            var account = accounts[i];

                            var confirmAccount = new BankBotConfirmAccount();

                            var cardToCardAccount = cardToCardAccounts.FirstOrDefault(t => t.Id == account.CardToCardAccountId);
                            confirmAccount.AccountGuid = cardToCardAccount.AccountGuid;
                            confirmAccount.CardNumber = EncryptionService.DecryptToString(cardToCardAccount.CardNumber);

                            if (!string.IsNullOrEmpty(confirmAccount.CardNumber))
                            {
                                var bankBotAccount = await _BankBotService.GetAccountByGuid(cardToCardAccount.AccountGuid);

                                confirmAccount.AccountNumber = bankBotAccount.AccountNo;
                                confirmAccount.BlockedDate = account.RelationStatus != CardToCardAccountGroupItemStatus.Blocked ? (DateTime?)null : account.BlockedDate;

                                accountList.Add(confirmAccount);
                            }
                        }

                        if (!accountList.Any(t => t.AccountGuid == transaction.AccountGuid))
                        {
                            accountList.Add(new BankBotConfirmAccount()
                            {
                                AccountGuid = transaction.AccountGuid,
                                AccountNumber = EncryptionService.DecryptToString(transaction.AccountNumber),
                                CardNumber = EncryptionService.DecryptToString(transaction.CardNumber),
                                BlockedDate = null
                            });
                        }

                        var real = accountList.FirstOrDefault(t => t.AccountGuid == transaction.AccountGuid);

                        if (real != null)
                        {
                            accountList.Remove(real);
                            accountList.Insert(0, real);
                        }

                        var requestContent = new BotConfirmRequest()
                        {
                            TransactionCode = request.Token,
                            SourceCardNumber = EncryptionService.DecryptToString(transaction.CustomerCardNumber),
                            DateOfTransaction = transaction.CreationDate,
                            TransactionId = transaction.Id,
                            Amount = transaction.TransactionAmount,
                            MinimumTransactionAmount = merchant.MinimumTransactionAmount,
                            CardToCardAccounts = accountList
                        };

                        var task = _BankBotService.Confirm(requestContent);

                        var confirmResult = await task;

                        if (confirmResult.IsPresentInStatement)
                        {
                            transaction.TransactionStatus = TransactionStatus.Completed;
                            transaction.UpdatedDate = DateTime.UtcNow;
                            transaction.BankNumber = string.Join(",", confirmResult.TransactonNumberOfStatements);
                            transaction.TransferredDate = confirmResult.TransactionDateTime;

                            await Manager.UpdateAsync(transaction);
                            await Manager.SaveAsync();

                            //if (transaction.MerchantCustomerId.HasValue)
                            //{
                            //    await MerchantCustomerManager.TransactionConfirmed(transaction.MerchantCustomerId, transaction.TransactionAmount);
                            //}

                            await AddToCallbackQueue(transaction);

                            await _BankStatementItemManger.UpdateStatementsWithTransaction(confirmResult.StatementIds, transaction.Id);

                            response.ResultCode = TransactionResultEnum.Success;
                            response.BankNumber = string.Join(",", confirmResult.TransactonNumberOfStatements);
                            return response;
                        }
                        else
                        {
                            await TransactionQueueService.InsertQueue(new BotQueueItem()
                            {
                                LastTryDateTime = null,
                                TransactionCode = request.Token,
                                TryCount = 0,
                                TenantGuid = transaction.TenantGuid
                            });
                        }
                    }

                    response.ResultCode = TransactionResultEnum.TransactionNotConfirmed;
                    return response;
                }

                return response;
            }
            catch (TransactionException ex)
            {
                Logger.LogError(ex, ex.Message);

                if (ex.Result == TransactionResultEnum.InvalidCardNumber && request != null)
                {
                    Logger.LogWarning(JsonConvert.SerializeObject(request));
                }
                return new CompletePaymentResponse(ex.Result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new CompletePaymentResponse(TransactionResultEnum.UnknownError);
            }
        }

        public override Task<bool> SendOtp(Transaction transaction, string cardNumber, string captcha)
        {
            throw new NotImplementedException();
        }
    }
}
