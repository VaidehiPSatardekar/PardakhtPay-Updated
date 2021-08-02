using System;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Domain.Dashboard;
using Pardakht.PardakhtPay.Shared.Extensions;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class OwnerBankLoginManager : BaseManager<OwnerBankLogin, IOwnerBankLoginRepository>, IOwnerBankLoginManager
    {
        IBankStatementItemRepository _BankStatementRepository;
        ICardToCardAccountRepository _CardToCardRepository;
        IAesEncryptionService _EncryptionService;
        ITimeZoneService _TimeZoneService = null;
        IWithdrawalRepository _WithdrawalRepository = null;

        public OwnerBankLoginManager(IOwnerBankLoginRepository repository,
            IBankStatementItemRepository bankStatementItemRepository,
            ICardToCardAccountRepository cardToCardAccountRepository,
            IAesEncryptionService aesEncryptionService,
            IWithdrawalRepository withdrawalRepository,
            ITimeZoneService timeZoneService) :base(repository)
        {
            _BankStatementRepository = bankStatementItemRepository;
            _CardToCardRepository = cardToCardAccountRepository;
            _EncryptionService = aesEncryptionService;
            _TimeZoneService = timeZoneService;
            _WithdrawalRepository = withdrawalRepository;
        }

        public OwnerBankLogin GetLoginWithGuid(string loginGuid)
        {
            return Repository.GetQuery(t => t.BankLoginGuid == loginGuid).FirstOrDefault();
        }

        public OwnerBankLogin GetLoginWithLoginRequestId(int loginRequestId)
        {
            return Repository.GetQuery(t => t.LoginRequestId == loginRequestId).FirstOrDefault();
        }

        public async Task<List<DailyAccountInformation>> GetDailyAccountInformations(DashboardQuery dashboardQuery)
        {

            string timeZoneCode = dashboardQuery.TimeZoneInfo.GetTimeZoneCode();

            var startDate = await _TimeZoneService.ConvertCalendar(DateTime.UtcNow.Date, Helper.Utc, timeZoneCode);

            var endDate = await _TimeZoneService.ConvertCalendar(DateTime.UtcNow.Date.AddDays(1), Helper.Utc, timeZoneCode);

            var query = Repository.GetQuery();

            var cardToCardQuery = _CardToCardRepository.GetQuery();

            //var bankStatementQuery = _BankStatementRepository.GetQuery(t => t.TransactionDateTime >= startDate && t.TransactionDateTime < endDate);
            var bankStatementQuery = (from b in _BankStatementRepository.GetQuery()
                                      where b.TransactionDateTime >= startDate && b.TransactionDateTime < endDate
                                      group new { b.Credit, b.Debit } by b.AccountGuid into g
                                      select new
                                      {
                                          AccountGuid = g.Key,
                                          Credit = g.Sum(p => (decimal?)p.Credit),
                                          Debit = g.Sum(p => (decimal?)p.Debit)
                                      });

            var pendingWithdrawalQuery = (from w in _WithdrawalRepository.GetQuery()
                                          where w.AccountGuid != null 
                                          && w.TransferStatus != (int)TransferStatus.Complete 
                                          && w.TransferStatus != (int)TransferStatus.AwaitingConfirmation
                                          && w.TransferStatus != (int)TransferStatus.FailedFromBank
                                          && w.TransferStatus != (int)TransferStatus.Invalid
                                          && w.TransferStatus != (int)TransferStatus.RefundFromBank
                                          && w.TransferStatus != (int)TransferStatus.InvalidIBANNumber
                                          && w.TransferStatus != (int)TransferStatus.AccountNumberInvalid
                                          && (w.WithdrawalStatus == (int)WithdrawalStatus.Pending || w.WithdrawalStatus == (int)WithdrawalStatus.PendingBalance || w.WithdrawalStatus == (int)WithdrawalStatus.Sent)
                                          group w.RemainingAmount by w.AccountGuid into g
                                          select new
                                          {
                                              AccountGuid = g.Key,
                                              Amount = (decimal?)g.Sum()
                                          });

            var q = (from o in query
                     join c in cardToCardQuery on o.BankLoginGuid equals c.LoginGuid
                     join s in bankStatementQuery on c.AccountGuid equals s.AccountGuid into emptyStatement
                     from s in emptyStatement.DefaultIfEmpty()
                     join w in pendingWithdrawalQuery on c.AccountGuid equals w.AccountGuid into emptyWithdrawal
                     from w in emptyWithdrawal.DefaultIfEmpty()
                     where o.IsDeleted == false && o.IsActive == true && c.IsActive == true
                     select new DailyAccountInformation()
                     {
                         AccountGuid = c.AccountGuid,
                         CardHolderName = c.CardHolderName,
                         CardNumber = c.CardNumber,
                         FriendlyName = o.FriendlyName,
                         LoginGuid = o.BankLoginGuid,
                         TotalDeposit = s.Credit ?? 0m,
                         TotalWithdrawal = s.Debit ?? 0m,
                         PendingWithdrawalAmount = w.Amount,
                         BankLoginId = o.BankLoginId
                     });

            var items = await _BankStatementRepository.GetModelItemsAsync(q);

            items.ForEach(item =>
            {
                item.CardHolderName = _EncryptionService.DecryptToString(item.CardHolderName);
                item.CardNumber = _EncryptionService.DecryptToString(item.CardNumber);
            });

            return items;
        }
    }
}
