using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using System.Numerics;
using Pardakht.PardakhtPay.Shared.Models.WebService.BankBot;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class ManualTransferManager : BaseManager<ManualTransfer, IManualTransferRepository>, IManualTransferManager
    {
        IManualTransferDetailRepository _DetailRepository = null;
        IBankBotService _BankBotService = null;
        IAesEncryptionService _EncryptionService = null;
        CurrentUser _CurrentUser;
        ITimeZoneService _TimeZoneService;
        IManualTransferDetailManager _DetailManager = null;
        IApplicationSettingService _ApplicationSettingService = null;
        IManualTransferSourceCardDetailsRepository _SourceCardDetailRepository = null;

        public ManualTransferManager(IManualTransferRepository repository,
            IBankBotService bankBotService,
            IAesEncryptionService aesEncryptionService,
            ITimeZoneService timeZoneService,
            CurrentUser currentUser,
            IManualTransferDetailManager detailManager,
            IManualTransferDetailRepository detailRepository,
            IApplicationSettingService applicationSettingService,
            IManualTransferSourceCardDetailsRepository sourceCardRepository) : base(repository)
        {
            _BankBotService = bankBotService;
            _EncryptionService = aesEncryptionService;
            _TimeZoneService = timeZoneService;
            _CurrentUser = currentUser;
            _DetailRepository = detailRepository;
            _DetailManager = detailManager;
            _ApplicationSettingService = applicationSettingService;
            _SourceCardDetailRepository = sourceCardRepository;

        }

        public async Task<List<ManualTransfer>> GetUnProcessedItemsAsync()
        {
            return await Repository.GetItemsAsync(t => t.Status == (int)ManualTransferStatus.Pending && (t.ImmediateTransfer == false && t.ExpectedTransferDate <= DateTime.UtcNow));
        }

        public async Task Check(DateTime startDate)
        {
            var query = Repository.GetQuery();
            var detailQuery = _DetailRepository.GetQuery();

            var partialSentItems = (from q in query
                                    where q.Status == (int)ManualTransferStatus.Processing
                                    && detailQuery.Any(p => p.ManualTransferId == q.Id && p.TransferStatus == (int)TransferStatus.NotSent)
                                    && detailQuery.Any(p => p.ManualTransferId == q.Id && p.TransferStatus != (int)TransferStatus.NotSent)
                                    && q.ProcessedDate >= startDate
                                    select q).ToList();

            for (int i = 0; i < partialSentItems.Count; i++)
            {
                var partialSent = partialSentItems[i];
                partialSent.Status = (int)ManualTransferStatus.PartialSent;
                partialSent.UpdateDate = DateTime.UtcNow;

                await UpdateAsync(partialSent);
                await SaveAsync();
            }

            var sentItems = (from q in query
                             where (q.Status == (int)ManualTransferStatus.Processing || q.Status == (int)ManualTransferStatus.PartialSent)
                             && !detailQuery.Any(p => p.ManualTransferId == q.Id && p.TransferStatus == (int)TransferStatus.NotSent)
                             && detailQuery.Any(p => p.ManualTransferId == q.Id && p.TransferStatus != (int)TransferStatus.NotSent)
                        && q.ProcessedDate >= startDate
                             select q).ToList();

            for (int i = 0; i < sentItems.Count; i++)
            {
                var sentItem = sentItems[i];
                sentItem.Status = (int)ManualTransferStatus.Sent;
                sentItem.UpdateDate = DateTime.UtcNow;

                await UpdateAsync(sentItem);
                await SaveAsync();
            }

            var partialCompletedItems = (from q in query
                                         where (q.Status == (int)ManualTransferStatus.Processing || q.Status == (int)ManualTransferStatus.PartialSent || q.Status == (int)ManualTransferStatus.Sent)
                                         && !detailQuery.Any(p => p.ManualTransferId == q.Id && p.TransferStatus == (int)TransferStatus.NotSent)
                                         && detailQuery.Any(p => p.ManualTransferId == q.Id && (p.TransferStatus == (int)TransferStatus.Complete || p.TransferStatus == (int)TransferStatus.CompletedWithNoReceipt))
                                         && detailQuery.Any(p => p.ManualTransferId == q.Id && p.TransferStatus != (int)TransferStatus.Complete && p.TransferStatus != (int)TransferStatus.CompletedWithNoReceipt)
                                           && q.ProcessedDate >= startDate
                                         select q).ToList();

            for (int i = 0; i < partialCompletedItems.Count; i++)
            {
                var partialCompleted = partialCompletedItems[i];
                partialCompleted.Status = (int)ManualTransferStatus.PartialCompleted;
                partialCompleted.UpdateDate = DateTime.UtcNow;

                await UpdateAsync(partialCompleted);
                await SaveAsync();
            }

            var completedItems = (from q in query
                                  where (q.Status == (int)ManualTransferStatus.Processing || q.Status == (int)ManualTransferStatus.PartialSent || q.Status == (int)ManualTransferStatus.PartialCompleted || q.Status == (int)ManualTransferStatus.Sent)
                                  && detailQuery.Any(p => p.ManualTransferId == q.Id && (p.TransferStatus == (int)TransferStatus.Complete || p.TransferStatus == (int)TransferStatus.CompletedWithNoReceipt))
                                  && !detailQuery.Any(p => p.ManualTransferId == q.Id && p.TransferStatus != (int)TransferStatus.Complete && p.TransferStatus != (int)TransferStatus.CompletedWithNoReceipt)
                        && q.ProcessedDate >= startDate
                                  select q).ToList();

            for (int i = 0; i < completedItems.Count; i++)
            {
                var completed = completedItems[i];
                completed.Status = (int)ManualTransferStatus.Completed;
                completed.UpdateDate = DateTime.UtcNow;

                await UpdateAsync(completed);
                await SaveAsync();
            }
        }

        public async Task Process(int id, List<BankBotAccountWithStatusDTO> accounts, List<BankBotBankInformation> banks, List<BankBotLoginInformation> logins)
        {
            var item = await GetEntityByIdAsync(id);

            if (item == null)
            {
                throw new Exception($"Manual Transfer could not be found with id {id}");
            }

            await Process(item, accounts, banks, logins);
        }

        public async Task Process(ManualTransfer transfer, List<BankBotAccountWithStatusDTO> accounts, List<BankBotBankInformation> banks, List<BankBotLoginInformation> logins, bool processDetails = false)
        {
            //string accountGuids = string.Join(",", transfer.AccountGuid); 
            var sourceAccountDetails = await _SourceCardDetailRepository.GetItemsAsync(s => s.ManualTransferId == transfer.Id);
            string accountGuids = "";
            
            foreach (var sourceTransferDetail in sourceAccountDetails)
            {
                accountGuids += sourceTransferDetail.AccountGuid + ",";
            }

            var accountList = accounts.Where(t => t.TransferType == transfer.TransferType && accountGuids.Contains(t.AccountGuid)).ToList();
            var amount = transfer.Amount;
            if (transfer.TransferWholeAmount)
            {
                amount = accountList.Select(x => x.WithdrawableBalance).Sum();
            }

            var configuration = await _ApplicationSettingService.Get<BankAccountConfiguration>();

            int i = 0;
            int DeletedCount = 0;
            int BlockedCount = 0;
            while (i < accountList.Count)
            {                
                if (!accountList[i].IsOpen(configuration.BlockAccountLimit))
                {
                    BlockedCount++;
                    //transfer.Status = (int)ManualTransferStatus.CancelledByBlockedAccount;
                    //await UpdateAsync(transfer);
                    //await SaveAsync();
                    continue;
                }
                if (accountList[i].IsDeleted)
                {
                    DeletedCount++;
                    //transfer.Status = (int)ManualTransferStatus.CancelledByDeletedAccount;
                    //await UpdateAsync(transfer);
                    //await SaveAsync();
                    continue;
                }

                var login = logins.FirstOrDefault(t => t.LoginGuid == accountList[i].LoginGuid);
                var bank = banks.FirstOrDefault(t => t.Id == login.BankId);

                int cnt = 0;
                while (amount >= accountList[i].MinimumWithdrawalOfEachTransaction && amount > 0 && accountList[i].WithdrawRemainedAmountForDay > 0 && accountList[i].WithdrawRemainedAmountForMonth > 0)
                {
                    cnt++;
                    var innerAmount = amount;

                    if (amount > accountList[i].MaximumWithdrawalOfEachTransaction)
                    {
                        innerAmount = accountList[i].MaximumWithdrawalOfEachTransaction;

                        var dif = amount - innerAmount;

                        if (dif < accountList[i].MinimumWithdrawalOfEachTransaction)
                        {
                            innerAmount = accountList[i].MinimumWithdrawalOfEachTransaction;
                        }
                    }

                    if (innerAmount > accountList[i].WithdrawRemainedAmountForDay)
                    {
                        innerAmount = accountList[i].WithdrawRemainedAmountForDay;
                    }

                    if (innerAmount > accountList[i].WithdrawRemainedAmountForMonth)
                    {
                        innerAmount = accountList[i].WithdrawRemainedAmountForMonth;
                    }

                    if (bank.ThresholdLimit.HasValue && innerAmount > bank.ThresholdLimit)
                    {
                        innerAmount = Convert.ToDecimal(bank.ThresholdLimit);
                    }

                    if (innerAmount % 10000000 != 0)
                    {
                        var coefficientAmount = BigInteger.Abs((BigInteger)(innerAmount / 10000000)) * 10000000;
                        Decimal.TryParse(coefficientAmount.ToString(), out decimal convertedvalue);
                        if (convertedvalue != 0)
                        {
                            innerAmount = convertedvalue;
                        }
                    }

                    if (innerAmount == 0 || cnt > 100)
                    {
                        break;
                    }

                    amount -= innerAmount;
                    accountList[i].WithdrawRemainedAmountForDay -= Convert.ToInt64(innerAmount);
                    accountList[i].WithdrawRemainedAmountForMonth -= Convert.ToInt64(innerAmount);

                    var detail = new ManualTransferDetail();
                    detail.Amount = innerAmount;
                    detail.ManualTransferId = transfer.Id;
                    detail.TransferRequestDate = DateTime.UtcNow;
                    detail.TransferStatus = (int)TransferStatus.NotSent;
                    detail.CreateDate = DateTime.UtcNow;

                    await _DetailRepository.InsertAsync(detail);
                    await SaveAsync();

                    if (processDetails)
                    {
                        await _DetailManager.Process(detail, accountList[i].AccountGuid);
                    }
                }

                transfer.ProcessedDate = DateTime.UtcNow;
                transfer.Status = (int)ManualTransferStatus.Processing;

                await UpdateAsync(transfer);

                await SaveAsync();
                i++;
            }

            if (DeletedCount == accountList.Count) {
                transfer.Status = (int)ManualTransferStatus.CancelledByDeletedAccount;
                await UpdateAsync(transfer);
                await SaveAsync();
            }
            else if (BlockedCount == accountList.Count) {
                transfer.Status = (int)ManualTransferStatus.CancelledByBlockedAccount;
                await UpdateAsync(transfer);
                await SaveAsync();
            }
            else if (BlockedCount + DeletedCount== accountList.Count) {
                transfer.Status = (int)ManualTransferStatus.CancelledByBlockedORDeletedAccount;
                await UpdateAsync(transfer);
                await SaveAsync();
            }

        }
        public async Task<ListSearchResponse<List<ManualTransferDTO>>> Search(ManualTransferSearchArgs args)
        {
            var query = Repository.GetQuery();

            var innerQuery = _SourceCardDetailRepository.GetQuery();

            var selectQuery = (from s in query
                               select new ManualTransferDTO()
                               {
                                   AccountGuid = s.AccountGuid,
                                   Amount = s.Amount,
                                   CancelledDate = s.CancelledDate,
                                   CardToCardAccountId = s.CardToCardAccountId,
                                   CreationDate = s.CreationDate,
                                   ExpectedTransferDate = s.ExpectedTransferDate,
                                   FirstName = s.FirstName,
                                   Iban = s.Iban,
                                   Id = s.Id,
                                   ImmediateTransfer = s.ImmediateTransfer,
                                   LastName = s.LastName,
                                   OwnerGuid = s.OwnerGuid,
                                   Priority = s.Priority,
                                   ProcessedDate = s.ProcessedDate,
                                   Status = s.Status,
                                   TenantGuid = s.TenantGuid,
                                   ToAccountNo = s.ToAccountNo,
                                   TransferAccountId = s.TransferAccountId,
                                   TransferType = s.TransferType,
                                   CancellerId = s.CancellerId,
                                   CreatorId = s.CreatorId,
                                   UpdaterId = s.UpdaterId,
                                   TransferWholeAmount = s.TransferWholeAmount
                               });

            var accounts = await _BankBotService.GetAccountsAsync();

            selectQuery = selectQuery.Where(t => innerQuery.Where(p => args.AccountGuids.Contains(p.AccountGuid)).Select(m => m.ManualTransferId).Contains(t.Id) || args.AccountGuids.Contains(t.AccountGuid));

            accounts = accounts.Where(t => args.AccountGuids.Contains(t.AccountGuid)).ToList();

            if (accounts.Any(t => !_CurrentUser.LoginGuids.Contains(t.LoginGuid)))
            {
                throw new UnauthorizedAccessException("You don't have permission to access these accounts");
            }


            //selectQuery = selectQuery.Where(t => args.AccountGuids.Any(m => t.AccountGuid.Contains(m)));

            ////resultsList = resultsList
            ////     .Where(r => r.FormCodes.Split(',')
            ////                  .Any(code => masterSet.Contains(code)))
            ////     .ToList();

            if (!string.IsNullOrEmpty(args.ToAccountNo))
            {
                var accountNo = _EncryptionService.EncryptToBase64(args.ToAccountNo);
                selectQuery = selectQuery.Where(t => t.ToAccountNo == accountNo);
            }

            if (args.TransferType.HasValue && args.TransferType != 0)
            {
                selectQuery = selectQuery.Where(t => t.TransferType == args.TransferType);
            }

            string timeZoneCode = args.TimeZoneInfo.GetTimeZoneCode();

            if (args.FromDate.HasValue)
            {
                DateTime date = await _TimeZoneService.ConvertCalendar(args.FromDate.Value.Date, timeZoneCode, Helper.Utc);
                selectQuery = selectQuery.Where(t => t.CreationDate >= date);
            }

            if (args.ToDate.HasValue)
            {
                DateTime toDate = await _TimeZoneService.ConvertCalendar(args.ToDate.Value.Date.AddDays(1), timeZoneCode, Helper.Utc);
                selectQuery = selectQuery.Where(t => t.CreationDate < toDate);
            }

            if (args.Status.HasValue && args.Status != 0)
            {
                selectQuery = selectQuery.Where(t => t.Status == args.Status);
            }

            if (args.FilterModel != null)
            {
                selectQuery = selectQuery.ApplyParameters(args.FilterModel, _EncryptionService);
            }

            var totalCount = selectQuery.Count();
            bool sort = false;

            if (!string.IsNullOrEmpty(args.SortColumn))
            {
                switch (args.SortColumn)
                {
                    case "id":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.Id);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.Id);
                        }
                        break;
                    case "amount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.Amount);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.Amount);
                        }
                        break;
                    case "priority":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.Priority);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.Priority);
                        }
                        break;
                    case "transferType":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.TransferType);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.TransferType);
                        }
                        break;
                }
            }

            if (!sort)
            {
                selectQuery = selectQuery.OrderByDescending(t => t.CreationDate);
            }

            selectQuery = selectQuery.Skip(args.StartRow).Take(args.EndRow - args.StartRow);

            var items = selectQuery.Distinct().ToList();

            List<DateTime> dates = items.Select(t => t.CreationDate).ToList();

            var usedDates = items.Select(t => t.ExpectedTransferDate ?? DateTime.UtcNow).ToList();

            var processedDates = items.Select(t => t.ProcessedDate ?? DateTime.UtcNow).ToList();

            var cancelledDates = items.Select(t => t.CancelledDate ?? DateTime.UtcNow).ToList();

            string calendarCode = args.TimeZoneInfo.GetCalendarCode();

            var convertedDates = _TimeZoneService.ConvertCalendarLocal(dates, string.Empty, calendarCode);

            var usedConvertedDates = _TimeZoneService.ConvertCalendarLocal(usedDates, string.Empty, calendarCode);

            var processedConvertedDates = _TimeZoneService.ConvertCalendarLocal(processedDates, string.Empty, calendarCode);

            var cancelledConvertedDates = _TimeZoneService.ConvertCalendarLocal(cancelledDates, string.Empty, calendarCode);

            await Task.WhenAll(convertedDates, usedConvertedDates, processedConvertedDates, cancelledConvertedDates);

            return await Task.Run(() =>
            {
                List<ManualTransferDTO> dtos = new List<ManualTransferDTO>();

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    item.CreationDateStr = convertedDates.Result[i];

                    if (item.ExpectedTransferDate != null)
                    {
                        item.ExpectedTransferDateStr = usedConvertedDates.Result[i];
                    }

                    if (item.ProcessedDate != null)
                    {
                        item.ProcessedDateStr = processedConvertedDates.Result[i];
                    }

                    if (item.CancelledDate != null)
                    {
                        item.CancelledDateStr = cancelledConvertedDates.Result[i];
                    }

                    item.ToAccountNo = _EncryptionService.DecryptToString(item.ToAccountNo);
                    item.Iban = _EncryptionService.DecryptToString(item.Iban);
                    item.FirstName = _EncryptionService.DecryptToString(item.FirstName);
                    item.LastName = _EncryptionService.DecryptToString(item.LastName);

                    var account = accounts.FirstOrDefault(t => t.AccountGuid == item.AccountGuid);

                    if (account != null)
                    {
                        item.BankAccountId = account.Id;
                        item.BankLoginId = account.LoginId;
                    }

                    dtos.Add(item);
                }

                return new ListSearchResponse<List<ManualTransferDTO>>()
                {
                    Items = dtos,
                    Success = true,
                    Paging = new PagingHeader(totalCount, 0, 0, 0)
                };

            });
        }

        public async Task<List<ManualTransferDetail>> GetDetails(int id)
        {
            var items = await _DetailRepository.GetItemsAsync(t => t.ManualTransferId == id);

            return items;
        }

        public async Task<ManualTransferDetail> GetDetail(int id)
        {
            return await _DetailRepository.GetItemAsync(t => t.Id == id);
        }

        public async Task<BankBotTransferReceiptResponse> GetTransferReceipt(int id)
        {
            var detail = await _DetailRepository.GetEntityByIdAsync(id);

            if (detail == null)
            {
                throw new Exception("Detail not found");
            }

            if (string.IsNullOrEmpty(detail.TrackingNumber))
            {
                throw new Exception("Tracking number not found");
            }

            var receipt = await _BankBotService.GetTransferReceipt(new BankBotTransferReceiptRequest()
            {
                TrackingNumber = detail.TrackingNumber
            });

            return receipt;
        }

        public async Task InsertSourceCardDetails(List<ManualTransferSourceCardDetails> sourceCardDetails)
        {
            int j = 0;
            while (j < sourceCardDetails.Count)
            {
                ManualTransferSourceCardDetails entity = new ManualTransferSourceCardDetails();
                entity.ManualTransferId = sourceCardDetails[j].ManualTransferId;
                entity.CardToCardAccountId = sourceCardDetails[j].CardToCardAccountId;
                entity.AccountGuid = sourceCardDetails[j].AccountGuid;
                await _SourceCardDetailRepository.InsertAsync(entity);
                await SaveAsync();
                j++;
            }
        }

        public async Task DeleteSourceCardDetails(int  manualTransferId)
        {

            var sourceCardDetails = await _SourceCardDetailRepository.GetItemsAsync(t => t.ManualTransferId == manualTransferId);
            foreach (var source in sourceCardDetails)
            {
                _SourceCardDetailRepository.Delete(source.Id);
            }
        }

    }
}
