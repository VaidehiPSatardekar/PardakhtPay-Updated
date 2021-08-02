using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.QueueAdder
{
    public class OperationManager : IHostedService
    {
        //IWithdrawalManager withdrawalManager;
        //ITransactionManager transactionManager;
        //ITransactionQueueService queueService;
        IServiceProvider serviceProvider;
        CurrentUser currentUser;

        DateTime startDate = DateTime.MaxValue;// new DateTime(2021, 1, 19, 20, 0, 0);

        public OperationManager(
        //    ITransactionManager transactionManager,
        //ITransactionQueueService queueService,
        //IWithdrawalManager withdrawalManager,
        IServiceProvider serviceProvider,
        CurrentUser currentUser)
        {
            //this.transactionManager = transactionManager;
            //this.queueService = queueService;
            //this.withdrawalManager = withdrawalManager;
            this.serviceProvider = serviceProvider;
            this.currentUser = currentUser;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            currentUser.ApiCall = true;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            
        }
    }
}
