using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.ExternalServices.Queue
{
    public class TransactionAzureQueueService : ITransactionQueueService
    {
        IConfiguration _Configuration = null;
        QueueConfiguration _QueueConfiguration = null;
        ILogger _Logger = null;

        public TransactionAzureQueueService(IOptions<QueueConfiguration> queueOptions,
            IConfiguration configuration,
            ILogger<TransactionAzureQueueService> logger)
        {
            _Configuration = configuration;
            _QueueConfiguration = queueOptions.Value;
            _Logger = logger;
        }

        public async Task InsertCallbackQueueItem(CallbackQueueItem queueItem, TimeSpan delay)
        {
            try
            {
                var connectionString = _Configuration.GetConnectionString(_QueueConfiguration.QueueConnectionStringName);

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var queueClient = storageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(_QueueConfiguration.CallbackQueueName);
                await queue.CreateIfNotExistsAsync();
                var insertMessage = new CloudQueueMessage(JsonConvert.SerializeObject(queueItem));

                await queue.AddMessageAsync(insertMessage, null, delay, null, null);
            }
            catch(Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        public async Task InsertCallbackQueueItem(CallbackQueueItem queueItem)
        {
            await InsertCallbackQueueItem(queueItem, _QueueConfiguration.CallbackDelay);
        }

        public async Task InsertMobileTransferQueueItem(MobileTransferQueueItem queueItem)
        {
            await InsertMobileTransferQueueItem(queueItem, _QueueConfiguration.MobileTransferDelay);
        }

        public async Task InsertMobileTransferQueueItem(MobileTransferQueueItem queueItem, TimeSpan delay)
        {
            try
            {
                var connectionString = _Configuration.GetConnectionString(_QueueConfiguration.QueueConnectionStringName);

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var queueClient = storageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(_QueueConfiguration.MobileTransferQueueName);
                await queue.CreateIfNotExistsAsync();
                var insertMessage = new CloudQueueMessage(JsonConvert.SerializeObject(queueItem));

                await queue.AddMessageAsync(insertMessage, null, delay, null, null);
            }
            catch(Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        public async Task InsertQueue(BotQueueItem queueItem, TimeSpan delay)
        {
            try
            {
                var connectionString = _Configuration.GetConnectionString(_QueueConfiguration.QueueConnectionStringName);

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var queueClient = storageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(_QueueConfiguration.QueueName);
                await queue.CreateIfNotExistsAsync();
                var insertMessage = new CloudQueueMessage(JsonConvert.SerializeObject(queueItem));

                await queue.AddMessageAsync(insertMessage, null, delay, null, null);
            }
            catch(Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        public async Task InsertQueue(BotQueueItem queueItem)
        {
            await InsertQueue(queueItem, _QueueConfiguration.Delay);
        }

        public async Task InsertWithdrawalCallbackQueueItem(WithdrawalQueueItem queueItem)
        {
            await InsertWithdrawalCallbackQueueItem(queueItem, _QueueConfiguration.WithdrawalCallbackDelay);
        }

        public async Task InsertWithdrawalCallbackQueueItem(WithdrawalQueueItem queueItem, TimeSpan delay)
        {
            try
            {
                var connectionString = _Configuration.GetConnectionString(_QueueConfiguration.QueueConnectionStringName);

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var queueClient = storageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(_QueueConfiguration.WithdrawalCallbackQueueName);
                await queue.CreateIfNotExistsAsync();
                var insertMessage = new CloudQueueMessage(JsonConvert.SerializeObject(queueItem));

                await queue.AddMessageAsync(insertMessage, null, delay, null, null);
                _Logger.LogInformation($"Callback added to queue for withdrawal {queueItem.Id}.");
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }
    }
}
