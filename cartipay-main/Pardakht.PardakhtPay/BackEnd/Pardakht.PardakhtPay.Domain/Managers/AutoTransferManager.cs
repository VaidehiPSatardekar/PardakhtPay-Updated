using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;
using Pardakht.PardakhtPay.Domain.Dashboard;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class AutoTransferManager : BaseManager<AutoTransfer, IAutoTransferRepository>, IAutoTransferManager
    {

        IAesEncryptionService _AesEncryptionService = null;
        ITimeZoneService _TimeZoneService = null;
        IOwnerBankLoginManager _OwnerBankLoginManager;
        IBankBotService _BankBotService;

        public AutoTransferManager(IAutoTransferRepository repository,
            IAesEncryptionService aesEncryptionService,
            IOwnerBankLoginManager ownerBankLoginManager,
            IBankBotService bankBotService,
            ITimeZoneService timeZoneService) : base(repository)
        {
            _AesEncryptionService = aesEncryptionService;
            _TimeZoneService = timeZoneService;
            _OwnerBankLoginManager = ownerBankLoginManager;
            _BankBotService = bankBotService;
        }

        public async Task<AutoTransfer> Cancel(AutoTransfer item)
        {
            item.CancelDate = DateTime.UtcNow;
            item.IsCancelled = true;

            var updated = await Repository.UpdateAsync(item);

            return updated;
        }

        public async Task<List<AutoTransfer>> GetUncompletedTransfers()
        {
            var items = await Repository.GetItemsAsync(t => t.IsCancelled == false && t.Status != (int)TransferStatus.Complete && t.Status != (int)TransferStatus.AwaitingConfirmation && t.Status != (int)TransferStatus.BankSubmitted && t.Status != (int)TransferStatus.CompletedWithNoReceipt);

            return items;
        }

        public async Task<List<AutoTransfer>> GetPendingTransfers()
        {
            var items = await Repository.GetItemsAsync(t => t.IsCancelled == false && t.Status != (int)TransferStatus.Complete && t.Status != (int)TransferStatus.AwaitingConfirmation && t.Status != (int)TransferStatus.BankSubmitted && t.Status != (int)TransferStatus.CompletedWithNoReceipt);

            return items;
        }

        public async Task<ListSearchResponse<IEnumerable<AutoTransferDTO>>> Search(AutoTransferSearchArgs args)
        {
            var query = Repository.GetQuery();

            var itemQuery = (from t in query
                             select new AutoTransferDTO()
                             {
                                 Id = t.Id,
                                 RequestGuid = t.RequestGuid,
                                 Priority = t.Priority,
                                 AccountGuid = t.AccountGuid,
                                 TransferToAccount = t.TransferToAccount,
                                 TransferRequestDate = t.TransferRequestDate,
                                 Amount = t.Amount,
                                 CancelDate = t.CancelDate,
                                 CardToCardAccountId = t.CardToCardAccountId,
                                 IsCancelled = t.IsCancelled,
                                 OwnerGuid = t.OwnerGuid,
                                 RequestId = t.RequestId,
                                 Status = t.Status,
                                 StatusDescription = t.StatusDescription,
                                 TenantGuid = t.TenantGuid,
                                 TransferredDate = t.TransferredDate,
                                 TransferFromAccount = t.TransferFromAccount
                             });

            if (args.Tenants != null)
            {
                itemQuery = itemQuery.Where(t => args.Tenants.Contains(t.TenantGuid));
            }

            DateTime? startDate = null;
            DateTime? endDate = null;

            //var today = TimeZoneInfo.ConvertTime(DateTime.Now, args.TimeZoneInfo).Date;

            var timeZoneCode = args.TimeZoneInfo.GetTimeZoneCode();

            var today = await _TimeZoneService.ConvertCalendar(DateTime.UtcNow, string.Empty, timeZoneCode);

            switch (args.DateRange)
            {
                case DatePeriodType.Today:
                    startDate = today.Date;
                    endDate = today.Date.AddDays(1);
                    break;
                case DatePeriodType.Yesterday:
                    startDate = today.Date.Yesterday();
                    endDate = today;
                    break;
                case DatePeriodType.ThisWeek:
                    startDate = today.ThisMonday();
                    endDate = startDate.Value.AddDays(7);
                    break;
                case DatePeriodType.LastWeek:
                    startDate = today.LastMonday();
                    endDate = today.ThisMonday();
                    break;
                case DatePeriodType.ThisMonth:
                    startDate = today.ThisMonthStart();
                    endDate = today.ThisMonthEnd();
                    break;
                case DatePeriodType.LastMonth:
                    startDate = today.LastMonthStart();
                    endDate = today.ThisMonthStart();
                    break;
                default:
                    break;
            }

            bool sort = false;

            if (!string.IsNullOrEmpty(args.SortColumn))
            {
                switch (args.SortColumn)
                {
                    case "statusDescription":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.StatusDescription);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.StatusDescription);
                        }
                        break;
                    case "transferRequestDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TransferRequestDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TransferRequestDate);
                        }
                        break;
                    case "transferredDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TransferredDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TransferredDate);
                        }
                        break;
                    case "amount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.Amount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.Amount);
                        }
                        break;
                    case "cancelDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.CancelDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.CancelDate);
                        }
                        break;
                    case "requestId":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.RequestId);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.RequestId);
                        }
                        break;
                    case "isCancelled":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.IsCancelled);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.IsCancelled);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (!sort)
            {
                itemQuery = itemQuery.OrderByDescending(t => t.TransferRequestDate);
            }

            if (startDate.HasValue)
            {
                startDate = await _TimeZoneService.ConvertCalendar(startDate.Value.Date, timeZoneCode, Helper.Utc);
                itemQuery = itemQuery.Where(t => t.TransferRequestDate >= startDate);
            }

            if (endDate.HasValue)
            {
                endDate = await _TimeZoneService.ConvertCalendar(endDate.Value.Date, timeZoneCode, Helper.Utc);
                itemQuery = itemQuery.Where(t => t.TransferRequestDate < endDate);
            }

            var totalCount = itemQuery.Count();

            List<AutoTransferDTO> items = null;

            //if (args.PageSize == 0)
            //{
            //    items = itemQuery.ToList();
            //}
            //else
            //{
            //    items = itemQuery.Skip(args.PageSize * (args.PageNumber)).Take(args.PageSize).ToList();
            //}

            items = itemQuery.Skip(args.StartRow).Take(args.EndRow - args.StartRow).ToList();

            List<DateTime> transferRequestDates = items.Select(t => t.TransferRequestDate).ToList();

            List<DateTime> transferDates = items.Select(t => t.TransferredDate ?? DateTime.UtcNow).ToList();

            List<DateTime> cancelDates = items.Select(t => t.CancelDate ?? DateTime.UtcNow).ToList();

            string calendarCode = args.TimeZoneInfo.GetCalendarCode();

            var transferRequestConvertedDates =  _TimeZoneService.ConvertCalendarLocal(transferRequestDates, string.Empty, calendarCode);

            var transferConvertedDates = _TimeZoneService.ConvertCalendarLocal(transferDates, string.Empty, calendarCode);

            var cancelConvertedDates =  _TimeZoneService.ConvertCalendarLocal(cancelDates, string.Empty, calendarCode);

            await Task.WhenAll(transferRequestConvertedDates, transferConvertedDates, cancelConvertedDates);

            var accounts = await _BankBotService.GetAccountsAsync();
            var logins = await _OwnerBankLoginManager.GetAllAsync();

            for(int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                item.TransferRequestDateStr = transferRequestConvertedDates.Result[i];

                if (item.TransferredDate != null)
                {
                    item.TransferredDateStr = transferConvertedDates.Result[i];
                }

                if (item.CancelDate != null)
                {
                    item.CancelDateStr = cancelConvertedDates.Result[i];
                }

                if (!string.IsNullOrEmpty(item.TransferFromAccount))
                {
                    item.TransferFromAccount = _AesEncryptionService.DecryptToString(item.TransferFromAccount);
                }
                if (!string.IsNullOrEmpty(item.TransferToAccount))
                {
                    item.TransferToAccount = _AesEncryptionService.DecryptToString(item.TransferToAccount);
                }

                var account = accounts.FirstOrDefault(t => t.AccountGuid == item.AccountGuid);

                if(account != null)
                {
                    item.BankAccountId = account.Id;
                    item.BankLoginId = account.LoginId;

                    var login = logins.FirstOrDefault(t => t.BankLoginGuid == account.LoginGuid);

                    if(login != null)
                    {
                        item.FriendlyName = login.FriendlyName;
                    }
                }
            }

            return new ListSearchResponse<IEnumerable<AutoTransferDTO>>()
            {
                Items = items.AsEnumerable(),
                Success = true,
                Paging = new PagingHeader(totalCount, 0, 0, 0)
            };
        }

        public async Task<AutoTransfer> GetLatestAutoTranferRecord(int cardToCardAccountId)
        {
            return await Repository.GetLatesAutoTranfer(cardToCardAccountId);
        }

        public async Task<AutoTransfer> CheckAutoTransferStatus(AutoTransfer item, bool force = false, int statementId = 0)
        {
                if (item.RequestId == 0)
                {
                    return item;
                }

                if (!force)
                {
                    if (item.RequestId == (int)TransferStatus.Complete || item.RequestId == (int)TransferStatus.RefundFromBank)
                    {
                        return item;
                    }
                }

                var response = await _BankBotService.GetTransferRequestWithStatus(item.RequestId);
                if (response.TransferStatus == (int)TransferStatus.RefundFromBank)
                {
                    item.Status = response.TransferStatus;
                    item = await UpdateAsync(item);
                    await SaveAsync();
                }
                else
                {
                    if (response.TransferStatus != item.Status)
                    {
                        item.Status = response.TransferStatus;
                        item.TransferRequestDate = response.TransferRequestDate;
                        item.RequestGuid = response.TransferRequestGuid;
                        item = await UpdateAsync(item);
                        await SaveAsync();
                    }
                }
                return item;
        }

    }
}
