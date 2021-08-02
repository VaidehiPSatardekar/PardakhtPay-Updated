using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.BotAutoTransferService
{
    public class OperationManager : IHostedService, IDisposable
    {
        IServiceProvider _Provider = null;
        ILogger<OperationManager> _Logger = null;
        AutoTransferSettings _Settings = null;
        WithdrawalConfiguration _WithdrawalConfiguration = null;
        ManualTransferConfiguration _ManualTransferConfiguration = null;
        PausedAccountConfiguration _PausedAccountsConfiguration = null;
        CardHolderNameConfiguration _CardHolderNameConfiguration = null;
        InvoiceConfiguration _InvoiceConfiguration = null;
        MobileTransferConfiguration _MobileTransferConfiguration = null;
        LoginDeviceConfiguration _LoginDeviceConfiguration = null;
        ProxyPaymentApiSettings _ProxyPaymentApiSettings = null;

        System.Timers.Timer _Timer = null;

        System.Timers.Timer _WithdrawalTimer = null;

        System.Timers.Timer _ManualTransferTimer = null;

        System.Timers.Timer _PausedAccountsTimer = null;

        System.Timers.Timer _CheckCardNumberTimer = null;

        System.Timers.Timer _CardHolderNameOperationTimer = null;

        System.Timers.Timer _InvoiceTimer = null;

        System.Timers.Timer _PardakhtPalTimer = null;

        System.Timers.Timer _LoginDeviceTimer = null;

        System.Timers.Timer _ProxyPaymentTimer = null;

        public OperationManager(IServiceProvider serviceProvider,
            ILogger<OperationManager> logger,
            IOptions<AutoTransferSettings> autoTransferOptions,
            IOptions<WithdrawalConfiguration> withdrawalOptions,
            IOptions<ManualTransferConfiguration> manualTranferOptions,
            IOptions<PausedAccountConfiguration> pausedAccountsOptions,
            IOptions<CardHolderNameConfiguration> cardHolderOptions,
            IOptions<InvoiceConfiguration> invoiceConfigurationOptions,
            IOptions<MobileTransferConfiguration> mobileTransferOptions,
            IOptions<LoginDeviceConfiguration> loginDeviceConfigurationOptions,
            IOptions<ProxyPaymentApiSettings> proxyPaymentApiSettingsOptions)
        {
            _Provider = serviceProvider;
            _Logger = logger;
            _Settings = autoTransferOptions.Value;
            _WithdrawalConfiguration = withdrawalOptions.Value;
            _ManualTransferConfiguration = manualTranferOptions.Value;
            _PausedAccountsConfiguration = pausedAccountsOptions.Value;
            _CardHolderNameConfiguration = cardHolderOptions.Value;
            _InvoiceConfiguration = invoiceConfigurationOptions.Value;
            _MobileTransferConfiguration = mobileTransferOptions.Value;
            _LoginDeviceConfiguration = loginDeviceConfigurationOptions.Value;
            _ProxyPaymentApiSettings = proxyPaymentApiSettingsOptions.Value;
        }

        public void Run()
        {
            if (_Settings.Enabled)
            {
                _Timer = new System.Timers.Timer();
                _Timer.Elapsed += DoOperations;
                _Timer.Interval = _Settings.TransferInterval.TotalMilliseconds;
                _Timer.Enabled = true;
                _Timer.Start();
            }

            if (_WithdrawalConfiguration.Enabled)
            {
                _WithdrawalTimer = new System.Timers.Timer();
                _WithdrawalTimer.Elapsed += DoWithdrawalOperations;
                _WithdrawalTimer.Interval = _WithdrawalConfiguration.TransferInterval.TotalMilliseconds;
                _WithdrawalTimer.Enabled = true;
                _WithdrawalTimer.Start();
            }

            if (_ManualTransferConfiguration.Enabled)
            {
                _ManualTransferTimer = new System.Timers.Timer();
                _ManualTransferTimer.Elapsed += DoManualTransferOperations;
                _ManualTransferTimer.Interval = _ManualTransferConfiguration.TransferInterval.TotalMilliseconds;
                _ManualTransferTimer.Enabled = true;
                _ManualTransferTimer.Start();
            }

            if (_PausedAccountsConfiguration.Enabled)
            {
                _PausedAccountsTimer = new System.Timers.Timer();
                _PausedAccountsTimer.Elapsed += DoPausedAccountsOperations;
                _PausedAccountsTimer.Interval = _PausedAccountsConfiguration.Interval.TotalMilliseconds;
                _PausedAccountsTimer.Enabled = true;
                _PausedAccountsTimer.Start();

                _CheckCardNumberTimer = new System.Timers.Timer();
                _CheckCardNumberTimer.Elapsed += DoCheckCardNumberOperations;
                _CheckCardNumberTimer.Interval = _PausedAccountsConfiguration.CheckCardNumberInterval.TotalMilliseconds;
                _CheckCardNumberTimer.Enabled = true;
                _CheckCardNumberTimer.Start();
            }

            if (_CardHolderNameConfiguration.Enabled)
            {
                _CardHolderNameOperationTimer = new System.Timers.Timer();
                _CardHolderNameOperationTimer.Elapsed += DoProcessCardHolderNameOperations;
                _CardHolderNameOperationTimer.Interval = _CardHolderNameConfiguration.Interval.TotalMilliseconds;
                _CardHolderNameOperationTimer.Enabled = true;
                _CardHolderNameOperationTimer.Start();
            }

            if (_InvoiceConfiguration.Enabled)
            {
                _InvoiceTimer = new System.Timers.Timer();
                _InvoiceTimer.Elapsed += DoProcessInvoiceOperations;
                _InvoiceTimer.Interval = _InvoiceConfiguration.Interval.TotalMilliseconds;
                _InvoiceTimer.Enabled = true;
                _InvoiceTimer.Start();
            }

            if (_MobileTransferConfiguration.Enabled)
            {
                _PardakhtPalTimer = new System.Timers.Timer();
                _PardakhtPalTimer.Elapsed += DoPardakhtPalOperations;
                _PardakhtPalTimer.Interval = _MobileTransferConfiguration.CheckInterval.TotalMilliseconds;
                _PardakhtPalTimer.Enabled = true;
                _PardakhtPalTimer.Start();
            }

            if (_ProxyPaymentApiSettings.EnabledSwitching)
            {
                _ProxyPaymentTimer = new System.Timers.Timer();
                _ProxyPaymentTimer.Elapsed += DoProxyPaymentSwitchingOperations;
                _ProxyPaymentTimer.Interval = _ProxyPaymentApiSettings.SwitchingInterval.TotalMilliseconds;
                _ProxyPaymentTimer.Enabled = true;
                _ProxyPaymentTimer.Start();
            }

            if (_LoginDeviceConfiguration.Enabled)
            {
                _LoginDeviceTimer = new System.Timers.Timer();
                _LoginDeviceTimer.Elapsed += DoLoginDeviceStatusOperations;
                _LoginDeviceTimer.Interval = _LoginDeviceConfiguration.Interval.TotalMilliseconds;
                _LoginDeviceTimer.Enabled = true;
                _LoginDeviceTimer.Start();

            }

            //var task = Task.Run(DoProcessCardHolderNameOperations);
            //task.Wait();
        }

        private void DoProxyPaymentSwitchingOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            var task = Task.Run(() => DoProxyPaymentSwitchingOperations());

            task.Wait();
        }

        private void DoPardakhtPalOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            var task = Task.Run(() => DoPardakhtPalOperations());
            task.Wait();
        }

        private void DoProcessInvoiceOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            var task = Task.Run(() => DoInvoiceOperations());
            task.Wait();
        }

        private void DoProcessCardHolderNameOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            var task = Task.Run(() => DoProcessCardHolderNameOperations());
            task.Wait();
        }

        private void DoCheckCardNumberOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            var task = Task.Run(() => DoCheckCardNumberOperations());
            task.Wait();
        }

        private void DoPausedAccountsOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            var task = Task.Run(() => DoPausedAccountsOperations());
            task.Wait();
        }

        private void DoManualTransferOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            var task = Task.Run(() => DoManualTransferManageOperations());
            task.Wait();
        }

        private void DoWithdrawalOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            var task = Task.Run(() => DoWithdrawalOperations());
            task.Wait();
        }

        private void DoOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            var task = Task.Run(() => DoOperations());
            task.Wait();
        }

        private void DoLoginDeviceStatusOperations(object sender, System.Timers.ElapsedEventArgs e)
        {
            var task = Task.Run(() => DoLoginDeviceStatusOperations());
            task.Wait();
        }

        private async Task DoOperations()
        {
            try
            {
                _Logger.LogInformation("Starting Transfers");

                await DoCheckOperations();

                await DoTransferOperations();

                _Logger.LogInformation("Ending Transfers");
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        private async Task DoCheckOperations()
        {
            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<ICheckTransferManager>();

                await manager.Run();
            }
        }

        private async Task DoTransferOperations()
        {
            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<IAutoTransferOperationManager>();

                await manager.Run();
            }
        }

        private async Task DoWithdrawalOperations()
        {
            try
            {
                _Logger.LogInformation("Starting Withdrawals");

                await DoWithdrawalCheckOperations();

                await DoWithdrawalTransferOperations();

                _Logger.LogInformation("Ending Withdrawals");
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        private async Task DoWithdrawalCheckOperations()
        {
            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<ICheckWithdrawalManager>();

                await manager.Run();
            }
        }

        private async Task DoWithdrawalTransferOperations()
        {
            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<IWithdrawalOperationManager>();

                await manager.Run();
            }
        }

        private async Task DoManualTransferManageOperations()
        {
            try
            {
                _Logger.LogInformation("Starting Manual Transfers");

                await DoManualTransferCheckOperations();

                await DoManualTransferOperations();

                _Logger.LogInformation("Ending Manual Transfers");
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        private async Task DoManualTransferCheckOperations()
        {
            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<ICheckManualTransferManager>();

                await manager.Run();
            }
        }

        private async Task DoManualTransferOperations()
        {
            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<IManualTransferOperationManager>();

                await manager.Run();
            }
        }

        private async Task DoPausedAccountsOperations()
        {
            _Logger.LogInformation("Paused Account Operations is started");
            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<ICheckPausedAccountsManager>();

                await manager.Run();
            }
            _Logger.LogInformation("Paused Account Operations is ended");
        }

        private async Task DoCheckCardNumberOperations()
        {
            _Logger.LogInformation("Checking card number is started");
            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<ICheckCardNumberManager>();

                await manager.Run();
            }
            _Logger.LogInformation("Checking card number is ended");
        }

        private async Task DoProcessCardHolderNameOperations()
        {
            _Logger.LogInformation("Card holder name is started");
            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<ICardHolderNameOperationManager>();

                await manager.Run();
            }
            _Logger.LogInformation("Card holder name is ended");
        }

        private async Task DoInvoiceOperations()
        {
            _Logger.LogInformation("Invoice operation is started");

            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<IInvoiceOperationManager>();

                await manager.Run();
            }

            _Logger.LogInformation("Invoice operations is ended");
        }

        private async Task DoLoginDeviceStatusOperations()
        {
            _Logger.LogInformation("Login device status check operation is started");
            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<ICheckLoginDeviceStatusManager>();

                await manager.Run();
            }
            _Logger.LogInformation("Login device status check operation is ended");
        }

        private async Task DoPardakhtPalOperations()
        {
            _Logger.LogInformation("PardakhtPal operation is started");

            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<IPardakhtPalOperationManager>();

                await manager.Run();
            }

            _Logger.LogInformation("PardakhtPal operations is ended");
        }

        private async Task DoProxyPaymentSwitchingOperations()
        {
            _Logger.LogInformation("Proxy payment operation is started");

            using (var scope = _Provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetRequiredService<CurrentUser>();
                user.ApiCall = true;

                var manager = scope.ServiceProvider.GetRequiredService<IProxyPaymentThresholdManager>();

                await manager.Run();
            }
            _Logger.LogInformation("Proxy payment operation is ended");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Run();

            _Logger.LogInformation("Service started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _Logger.LogInformation("Service stopped");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_Timer != null)
            {
                _Timer.Enabled = false;
                _Timer.Dispose();
                _Timer = null;
            }

            if (_ManualTransferTimer != null)
            {
                _ManualTransferTimer.Enabled = false;
                _ManualTransferTimer.Dispose();
                _ManualTransferTimer = null;
            }

            if (_WithdrawalTimer != null)
            {
                _WithdrawalTimer.Enabled = false;
                _WithdrawalTimer.Dispose();
                _WithdrawalTimer = null;
            }

            if (_CheckCardNumberTimer != null)
            {
                _CheckCardNumberTimer.Enabled = false;
                _CheckCardNumberTimer.Dispose();
                _CheckCardNumberTimer = null;
            }

            if (_PausedAccountsTimer != null)
            {
                _PausedAccountsTimer.Enabled = false;
                _PausedAccountsTimer.Dispose();
                _PausedAccountsTimer = null;
            }

            if (_CardHolderNameOperationTimer != null)
            {
                _CardHolderNameOperationTimer.Enabled = false;
                _CardHolderNameOperationTimer.Dispose();
                _CardHolderNameOperationTimer = null;
            }

            if (_InvoiceTimer != null)
            {
                _InvoiceTimer.Enabled = false;
                _InvoiceTimer.Dispose();
                _InvoiceTimer = null;
            }

            if (_PardakhtPalTimer != null)
            {
                _PardakhtPalTimer.Enabled = false;
                _PardakhtPalTimer.Dispose();
                _PardakhtPalTimer = null;
            }

            if (_LoginDeviceTimer != null)
            {
                _LoginDeviceTimer.Enabled = false;
                _LoginDeviceTimer.Dispose();
                _LoginDeviceTimer = null;
            }


            if (_ProxyPaymentTimer != null)
            {
                _ProxyPaymentTimer.Enabled = false;
                _ProxyPaymentTimer.Dispose();
                _ProxyPaymentTimer = null;
            }
        }
    }
}
