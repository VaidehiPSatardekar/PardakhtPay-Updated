using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.BotAutoTransferService.OperationManagers
{
    public class InvoiceOperationManager : IInvoiceOperationManager
    {
        IInvoiceManager _InvoiceManager = null;
        IInvoiceDetailManager _InvoiceDetailManager = null;
        IInvoiceOwnerSettingManager _InvoiceOwnerSettingManager = null;
        ITransactionManager _TransactionManager = null;
        IWithdrawalManager _WithdrawalManager = null;
        IWithdrawalTransferHistoryManager _WithdrawalTransferHistoryManager = null;
        IMerchantManager _MerchantManager = null;
        ILogger _Logger = null;

        public InvoiceOperationManager(IInvoiceManager manager,
            IInvoiceDetailManager detailManager,
            IInvoiceOwnerSettingManager invoiceOwnerSettingManager,
            ITransactionManager transactionManager,
            IWithdrawalManager withdrawalManager,
            IWithdrawalTransferHistoryManager withdrawalTransferHistoryManager,
            IMerchantManager merchantManager,
            ILogger<InvoiceOperationManager> logger)
        {
            _InvoiceManager = manager;
            _InvoiceDetailManager = detailManager;
            _InvoiceOwnerSettingManager = invoiceOwnerSettingManager;
            _TransactionManager = transactionManager;
            _WithdrawalManager = withdrawalManager;
            _WithdrawalTransferHistoryManager = withdrawalTransferHistoryManager;
            _MerchantManager = merchantManager;
            _Logger = logger;
        }

        public async Task Run()
        {
            var invoiceOwnerSettings = await _InvoiceOwnerSettingManager.GetItemsAsync(t => t.IsActive);

            var merchants = await _MerchantManager.GetAllAsync();

            for (int i = 0; i < invoiceOwnerSettings.Count; i++)
            {
                var ownerSettings = invoiceOwnerSettings[i];

                try
                {
                    var lastInvoice = await _InvoiceManager.GetLastInvoiceAsync(ownerSettings.OwnerGuid);

                    DateTime startDate = ownerSettings.StartDate;
                    DateTime lastDate = ownerSettings.StartDate;
                    bool calculate = false;

                    if (lastInvoice != null)
                    {
                        startDate = lastInvoice.EndDate;
                        lastDate = lastInvoice.EndDate;
                    }

                    if (startDate >= DateTime.UtcNow)
                    {
                        break;
                    }

                    var period = (InvoicePeriods)ownerSettings.InvoicePeriod;

                    switch (period)
                    {
                        case InvoicePeriods.Daily:
                            if (DateTime.UtcNow.Subtract(lastDate).TotalDays > 1)
                            {
                                lastDate = lastDate.AddDays(1);
                                calculate = true;
                            }
                            break;
                        case InvoicePeriods.Weekly:
                            var day = lastDate.DayOfWeek;
                            var additionalDay = 0;
                            switch (day)
                            {
                                case DayOfWeek.Friday:
                                    additionalDay = 3;
                                    break;
                                case DayOfWeek.Monday:
                                    additionalDay = 7;
                                    break;
                                case DayOfWeek.Saturday:
                                    additionalDay = 2;
                                    break;
                                case DayOfWeek.Sunday:
                                    additionalDay = 1;
                                    break;
                                case DayOfWeek.Thursday:
                                    additionalDay = 4;
                                    break;
                                case DayOfWeek.Tuesday:
                                    additionalDay = 6;
                                    break;
                                case DayOfWeek.Wednesday:
                                    additionalDay = 5;
                                    break;
                                default:
                                    break;
                            }
                            lastDate = lastDate.AddDays(additionalDay);
                            calculate = true;
                            break;
                        case InvoicePeriods.Monthly:
                            lastDate = new DateTime(lastDate.Year, lastDate.Month, 1).AddMonths(1);
                            calculate = true;
                            break;
                        default:
                            break;
                    }

                    if (lastDate >= DateTime.UtcNow)
                    {
                        continue;
                    }

                    if (calculate)
                    {
                        var details = new List<InvoiceDetail>();

                        var transactions = await _TransactionManager.GetInvoiceDetails(ownerSettings.OwnerGuid, startDate, lastDate);

                        transactions = transactions.Where(t => t.TotalAmount > 0).ToList();

                        transactions.ForEach(transaction =>
                        {
                            var rate = 0m;

                            var type = (InvoiceDetailItemType)transaction.ItemTypeId;

                            switch (type)
                            {
                                case InvoiceDetailItemType.PardakhtPayDeposit:
                                    rate = ownerSettings.PardakhtPayDepositRate;
                                    break;
                                case InvoiceDetailItemType.PardakhtPalDeposit:
                                    rate = ownerSettings.PardakhtPalDepositRate;
                                    break;
                                case InvoiceDetailItemType.PardakhtPalWithdrawal:
                                    rate = ownerSettings.PardakhtPalWithdrawalRate;
                                    break;
                                case InvoiceDetailItemType.Withdrawal:
                                    rate = ownerSettings.WithdrawalRate;
                                    break;
                                default:
                                    throw new Exception($"Invocie detail type could not be found. {transaction.ItemTypeId}");
                            }

                            transaction.Amount = transaction.TotalAmount * rate;
                            transaction.Rate = rate;

                            details.Add(transaction);
                        });

                        var withdrawalDetails = await _WithdrawalTransferHistoryManager.GetInvoiceDetails(ownerSettings.OwnerGuid, startDate, lastDate);

                        withdrawalDetails = withdrawalDetails.Where(t => t.TotalAmount > 0).ToList();

                        withdrawalDetails.ForEach(withdrawalDetail =>
                        {
                            withdrawalDetail.Rate = ownerSettings.WithdrawalRate;
                            withdrawalDetail.Amount = withdrawalDetail.TotalAmount * ownerSettings.WithdrawalRate;

                            details.Add(withdrawalDetail);
                        });

                        details = details.Where(t => t.Amount > 0).ToList();

                        var invoice = new Invoice();
                        invoice.EndDate = lastDate;
                        invoice.OwnerGuid = ownerSettings.OwnerGuid;
                        invoice.StartDate = startDate;
                        invoice.Amount = details.Sum(t => t.Amount);
                        invoice.TenantGuid = ownerSettings.TenantGuid;

                        await _InvoiceManager.AddInvoice(invoice, details);
                    }
                }
                catch (Exception ex)
                {
                    _Logger.LogError(ex, $"Error while processing invoice for { JsonConvert.SerializeObject(ownerSettings) }");
                }
            }
        }
    }
}
