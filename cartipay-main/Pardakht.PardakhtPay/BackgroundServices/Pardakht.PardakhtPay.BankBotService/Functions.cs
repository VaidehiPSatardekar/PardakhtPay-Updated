using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Tenant;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.BankBotWebJobService
{
    public class Functions
    {
        QueueConfiguration _QueueConfiguration = null;
        IConfiguration _Configuration = null;
        const string QueueName = "cartipay";
        const string CallbackQueueName = "callback";
        const string WithdrawalCallbackQueueName = "withdrawalcallback";
        const string MobileTransferCallbackQueueName = "mobile";
        IServiceProvider _Provider = null;
        IHttpClientFactory _HttClientFactory = null;

        public Functions(IOptions<QueueConfiguration> options,
            IConfiguration configuration,
            IServiceProvider provider,
            IHttpClientFactory httpClientFactory)
        {
            _QueueConfiguration = options.Value;
            _Configuration = configuration;
            _Provider = provider;
            _HttClientFactory = httpClientFactory;
        }

        public async Task ProcessWorkItem(
            [QueueTrigger(QueueName)] BotQueueItem queueItem, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Processing bank bot queue item {queueItem.TransactionCode}");

                using (var scope = _Provider.CreateScope())
                {
                    var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                    user.ApiCall = true;

                    queueItem.TryCount++;
                    try
                    {
                        var bankBoatService = scope.ServiceProvider.GetRequiredService<IBankBotService>();
                        var response = await bankBoatService.Confirm(queueItem.TransactionCode);

                        if (!response.IsPresentInStatement)
                        {

                            if (queueItem.TryCount >= _QueueConfiguration.MaxTryCount)
                            {
                                logger.LogInformation($"Transaction could not be confirmed. Transaction Code : {queueItem.TransactionCode}. Try Count : {queueItem.TryCount}. Response : {JsonConvert.SerializeObject(response)}");

                                var transactionManager = scope.ServiceProvider.GetRequiredService<ITransactionManager>();
                                var transaction = await transactionManager.GetTransactionByToken(queueItem.TransactionCode);

                                transaction.TransactionStatus = TransactionStatus.Expired;
                                transaction.TransferredDate = null;
                                transaction.UpdatedDate = DateTime.UtcNow;
                                transaction.BankNumber = null;

                                await transactionManager.UpdateAsync(transaction);
                                await transactionManager.SaveAsync();
                            }
                            else
                            {
                                var queueService = scope.ServiceProvider.GetRequiredService<ITransactionQueueService>();

                                await queueService.InsertQueue(queueItem);
                            }
                        }
                        else
                        {
                            var transactionManager = scope.ServiceProvider.GetRequiredService<ITransactionManager>();
                            var transaction = await transactionManager.GetTransactionByToken(queueItem.TransactionCode);

                            if (transaction.TransactionStatus != TransactionStatus.Completed)
                            {
                                transaction.TransactionStatus = TransactionStatus.Completed;
                                transaction.TransferredDate = response.TransactionDateTime;
                                transaction.UpdatedDate = DateTime.UtcNow;
                                transaction.BankNumber = string.Join(",", response.TransactonNumberOfStatements);

                                await transactionManager.UpdateAsync(transaction);
                                await transactionManager.SaveAsync();

                                var statementManager = scope.ServiceProvider.GetRequiredService<IBankStatementItemManager>();

                                await statementManager.UpdateStatementsWithTransaction(response.StatementIds, transaction.Id);

                                var statusCode = await InformPardakhtPay(scope.ServiceProvider, queueItem.TransactionCode, transaction.TenantGuid);

                                if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Accepted)
                                {
                                    var queueService = scope.ServiceProvider.GetRequiredService<ITransactionQueueService>();

                                    logger.LogInformation($"Unsuccessful response from merchant or pardakhtpay when calling callback url {statusCode}. Token {transaction.Token}");

                                    await queueService.InsertCallbackQueueItem(new CallbackQueueItem()
                                    {
                                        LastTryDateTime = null,
                                        TryCount = 0,
                                        TenantGuid = transaction.TenantGuid,
                                        TransactionCode = transaction.Token
                                    });
                                }
                                else
                                {
                                    logger.LogInformation($"Callback is successfull {statusCode}. Token {transaction.Token}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (queueItem.TryCount < _QueueConfiguration.MaxTryCount)
                        {
                            var queueService = scope.ServiceProvider.GetRequiredService<ITransactionQueueService>();
                            await queueService.InsertQueue(queueItem);

                            logger.LogError(ex, ex.Message);
                        }
                    }
                }

                logger.LogInformation($"Processed bank bot queue item {queueItem.TransactionCode}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        public async Task ProcessCallbackWorkItem(
            [QueueTrigger(CallbackQueueName)] CallbackQueueItem queueItem, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Processing callback queue item {queueItem.TransactionCode}");
                queueItem.TryCount++;

                using (var scope = _Provider.CreateScope())
                {
                    try
                    {
                        var statusCode = await InformPardakhtPay(scope.ServiceProvider, queueItem.TransactionCode, queueItem.TenantGuid);

                        //var unauthorized = statusCode == HttpStatusCode.Unauthorized;

                        //if (unauthorized)
                        //{
                        //    var service = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
                        //    await service.Login();
                        //    statusCode = await InformPardakhtPay(scope.ServiceProvider, queueItem.TransactionCode, queueItem.TenantGuid);
                        //}

                        if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Accepted)
                        {
                            if (queueItem.TryCount < _QueueConfiguration.MaxCallbackTryCount)
                            {
                                var queueService = scope.ServiceProvider.GetRequiredService<ITransactionQueueService>();
                                await queueService.InsertCallbackQueueItem(queueItem);
                            }

                            logger.LogWarning($"PCWI : Unsuccessful response from merchant or pardakhtpay when calling callback url {statusCode}. Token {queueItem.TransactionCode}");
                        }
                        else
                        {
                            logger.LogInformation($"PCWI : Callback is successfull {statusCode}. Token {queueItem.TransactionCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (queueItem.TryCount < _QueueConfiguration.MaxCallbackTryCount)
                        {
                            var queueService = scope.ServiceProvider.GetRequiredService<ITransactionQueueService>();
                            await queueService.InsertCallbackQueueItem(queueItem);
                        }

                        logger.LogError(ex, ex.Message + " " + queueItem.TransactionCode);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        public async Task ProcessWithdrawalCallbackWorkItem(
            [QueueTrigger(WithdrawalCallbackQueueName)] WithdrawalQueueItem queueItem, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Processing withdrawal callback queue item {queueItem.Id}");
                queueItem.TryCount++;

                using (var scope = _Provider.CreateScope())
                {
                    try
                    {
                        var statusCode = await InformPardakhtPayForWithdrawal(scope.ServiceProvider, queueItem.Id, queueItem.TenantGuid);

                        //var unauthorized = statusCode == HttpStatusCode.Unauthorized;

                        //if (unauthorized)
                        //{
                        //    var service = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
                        //    await service.Login();
                        //    statusCode = await InformPardakhtPayForWithdrawal(scope.ServiceProvider, queueItem.Id, queueItem.TenantGuid);
                        //}

                        if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Accepted)
                        {
                            if (queueItem.TryCount < _QueueConfiguration.MaxWithdrawalCallbackTryCount)
                            {
                                var queueService = scope.ServiceProvider.GetRequiredService<ITransactionQueueService>();
                                await queueService.InsertWithdrawalCallbackQueueItem(queueItem);
                            }

                            logger.LogWarning($"PWCWI : Unsuccessful response from merchant or pardakhtpay when calling withdrawal callback url {statusCode}. Id {queueItem.Id}");
                        }
                        else
                        {
                            logger.LogInformation($"PWCWI : Withdrawal callback is successfull {statusCode}. Id {queueItem.Id}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (queueItem.TryCount < _QueueConfiguration.MaxWithdrawalCallbackTryCount)
                        {
                            var queueService = scope.ServiceProvider.GetRequiredService<ITransactionQueueService>();
                            await queueService.InsertWithdrawalCallbackQueueItem(queueItem);
                        }

                        logger.LogError(ex, ex.Message + " " + queueItem.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        public async Task ProcessMobileTransferWorkItem(
            [QueueTrigger(MobileTransferCallbackQueueName)] MobileTransferQueueItem queueItem, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Processing mobile transfer queue item {queueItem.TransactionCode}");
                queueItem.TryCount++;

                using (var scope = _Provider.CreateScope())
                {
                    try
                    {
                        var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                        user.ApiCall = true;

                        //ITenantResolver tenantResolver = scope.ServiceProvider.GetRequiredService<ITenantResolver>();

                        //var tenant = await tenantResolver.ResolveByGuid(queueItem.TenantGuid);

                        var transaction = await scope.ServiceProvider.GetRequiredService<ITransactionManager>().GetTransactionByToken(queueItem.TransactionCode);

                        var cachedObjectManager = scope.ServiceProvider.GetRequiredService<ICachedObjectManager>();

                        var cardAccounts = await cachedObjectManager.GetCachedItems<CardToCardAccount, ICardToCardAccountRepository>();

                        var bankBotService = scope.ServiceProvider.GetRequiredService<IBankBotService>();

                        var logins = await bankBotService.GetLogins();

                        var account = cardAccounts.FirstOrDefault(t => t.CardNumber == transaction.CardNumber && logins.Any(p => p.LoginGuid == t.LoginGuid && !p.IsDeleted));

                        if (account != null && !string.IsNullOrEmpty(account.AccountGuid))
                        {
                            var result = await scope.ServiceProvider.GetRequiredService<IBankBotService>().SendMobileTransferInformation(new Shared.Models.WebService.MobileTransfer.MobileTransactionDTO()
                            {
                                Amount = transaction.TransactionAmount,
                                CardNumber = scope.ServiceProvider.GetRequiredService<IAesEncryptionService>().DecryptToString(transaction.CustomerCardNumber),
                                Token = transaction.Token,
                                TransactionNo = transaction.BankNumber,
                                UTCTransactionDateTime = transaction.UpdatedDate.Value,
                                AccountGuid = Guid.Parse(account.AccountGuid)
                            });

                            if (!result)
                            {
                                if (queueItem.TryCount < _QueueConfiguration.MaxMobileTransferTryCount)
                                {
                                    var queueService = scope.ServiceProvider.GetRequiredService<ITransactionQueueService>();
                                    await queueService.InsertMobileTransferQueueItem(queueItem);
                                }

                                logger.LogWarning($"MT : Unsuccessful response from bankbot for mobile transfers. Token {queueItem.TransactionCode}");
                            }
                            else
                            {
                                logger.LogInformation($"MT : Mobile transfer is successfull. Token {queueItem.TransactionCode}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (queueItem.TryCount < _QueueConfiguration.MaxMobileTransferTryCount)
                        {
                            var queueService = scope.ServiceProvider.GetRequiredService<ITransactionQueueService>();
                            await queueService.InsertMobileTransferQueueItem(queueItem);
                        }

                        logger.LogError(ex, ex.Message + " " + queueItem.TransactionCode);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Informs PardakhtPay when the transaction is confirmed or expired
        /// </summary>
        /// <param name="provider">Current scope provider</param>
        /// <param name="token">Token of the transaction which pardakhtpay will be informed about</param>
        /// <returns>If response status is Unauthorized(401) returns true. Otherwise returns false</returns>
        private async Task<HttpStatusCode> InformPardakhtPay(IServiceProvider provider, string token, string tenantGuid)
        {
            try
            {
                var functions = provider.GetRequiredService<IGenericManagementFunctions<PardakhtPayAuthenticationSettings>>();

                CheckTransactionRequest request = new CheckTransactionRequest();
                request.Token = token;

                using (var response = await functions.MakeRequest(request, null, "/api/transaction/paymentcheckcompleted", HttpMethod.Post))
                {
                    return response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Informs pardakhtpay when the transaction is confirmed or expired
        /// </summary>
        /// <param name="provider">Current scope provider</param>
        /// <param name="id">Identity value of Withdrawal record</param>
        /// <returns>If response status is Unauthorized(401) returns true. Otherwise returns false</returns>
        private async Task<HttpStatusCode> InformPardakhtPayForWithdrawal(IServiceProvider provider, int id, string tenantGuid)
        {
            try
            {
                var functions = provider.GetRequiredService<IGenericManagementFunctions<PardakhtPayAuthenticationSettings>>();

                using (var response = await functions.MakeRequest(null, null, $"/api/withdrawal/withdrawalcheckcompleted/{id}", HttpMethod.Post))
                {
                    return response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
