using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Dashboard.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Dashboard.Builders
{
    public class AccountStatusWidgetBuilder : WidgetBuilder, IAccountStatusWidgetBuilder
    {
        IOwnerBankLoginManager _OwnerBankLoginManager;
        IBankBotService _BankBotService;
        ICardToCardAccountManager _CardToCardAccountManager = null;

        public AccountStatusWidgetBuilder(IOwnerBankLoginManager ownerBankLoginManager,
            IBankBotService bankBotService,
            ICardToCardAccountManager cardToCardAccountManager)
        {
            _OwnerBankLoginManager = ownerBankLoginManager;
            _BankBotService = bankBotService;
            _CardToCardAccountManager = cardToCardAccountManager;
        }

        public override async Task<DashboardWidget> Build(DashboardQuery query)
        {
            var widget = new DashboardWidget()
            {
                Data = new WidgetData { CoreData = new List<WidgetDataCore>(), Extra = new List<WidgetDataCore>() }
            };
            var ownerLogins = await _OwnerBankLoginManager.GetDailyAccountInformations(query);

            var logins = await _BankBotService.GetLoginSelect();

            var accounts = await _BankBotService.GetAccountsWithStatus();

            var banks = await _BankBotService.GetBanks();

            var items = (from l in logins
                         join o in ownerLogins on l.LoginGuid equals o.LoginGuid
                         join b in banks on l.BankId equals b.Id
                         join a in accounts on l.LoginGuid equals a.LoginGuid
                         group a by new { Login = l, Bank = b, FriendlyName = o.FriendlyName, AccountNo = a.AccountNo, Status = a.StatusDescription, Balance = a.Balance, TotalDepositToday = o.TotalDeposit, TotalWithdrawalToday = o.TotalWithdrawal, CardNo = o.CardNumber, CardHolderName = o.CardHolderName } into xxGroup
                         select new AccountStatusItem
                         {
                             FriendlyName = xxGroup.Key.FriendlyName,
                             IsBlocked = xxGroup.Key.Login.IsBlocked,
                             AccountNo = xxGroup.Key.AccountNo,
                             BankName = xxGroup.Key.Bank.BankName,
                             AccountStatus = xxGroup.Key.Status,
                             AccountBalance = xxGroup.Key.Balance,
                             NormalWithdrawable = xxGroup.Where(t => t.TransferType == (int)TransferType.Normal).Sum(p => p.WithdrawRemainedAmountForDay),
                             SatnaWithdrawable = xxGroup.Where(t => t.TransferType == (int)TransferType.Satna).Sum(p => p.WithdrawRemainedAmountForDay),
                             PayaWithdrawable = xxGroup.Where(t => t.TransferType == (int)TransferType.Paya).Sum(p => p.WithdrawRemainedAmountForDay),
                             TotalDepositToday = Convert.ToInt64(xxGroup.Key.TotalDepositToday),
                             TotalWithdrawalToday = Convert.ToInt64(xxGroup.Key.TotalWithdrawalToday),
                             CardNo = xxGroup.Key.CardNo,
                             CardHolderName = xxGroup.Key.CardHolderName
                         }).OrderBy(t => t.FriendlyName).ToList();

            items.ForEach(item =>
            {
                var widgetDataCore = new WidgetDataCore()
                {
                    Label = item.AccountNo,
                    Count = new Dictionary<string, object>()
                    {
                        {
                            "AccountNo", item.AccountNo
                        },
                        {
                            "BankName", item.BankName
                        },
                        {
                            "IsBlocked", item.IsBlocked.ToString()
                        },
                        {
                            "FriendlyName", item.FriendlyName
                        },
                        {
                            "Status", item.AccountStatus
                        },
                        {
                            "CardHolderName", item.CardHolderName
                        },
                        {
                            "CardNumber", item.CardNo
                        },
                        {
                            "AccountBalance", item.AccountBalance.ToString()
                        },
                        {
                            "NormalWithdrawable", item.NormalWithdrawable.ToString()
                        },
                        {
                            "PayaWithdrawable", item.PayaWithdrawable.ToString()
                        },
                        {
                            "SatnaWithdrawable", item.SatnaWithdrawable.ToString()
                        },
                        {
                            "TotalDepositToday", item.TotalDepositToday.ToString()
                        },
                        {
                            "TotalWithdrawalToday", item.TotalWithdrawalToday.ToString()
                        }
                        }
                };

                widget.Data.CoreData.Add(widgetDataCore);
            });

            return widget;
        }
    }

    class AccountStatusItem
    {
        public string BankName { get; set; }

        public string FriendlyName { get; set; }

        public string CardHolderName { get; set; }

        public bool IsBlocked { get; set; }

        public string AccountStatus { get; set; }

        public string AccountNo { get; set; }

        public string CardNo { get; set; }

        public long AccountBalance { get; set; }

        public long NormalWithdrawable { get; set; }

        public long PayaWithdrawable { get; set; }

        public long SatnaWithdrawable { get; set; }

        public long TotalDepositToday { get; set; }

        public long TotalWithdrawalToday { get; set; }
    }
}
