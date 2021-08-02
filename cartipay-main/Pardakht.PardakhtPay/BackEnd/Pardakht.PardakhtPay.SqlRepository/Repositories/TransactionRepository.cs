using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(PardakhtPayDbContext context,
            IServiceProvider provider) :base(context, provider)
        {

        }

        public async Task<List<InvoiceDetail>> GetInvoiceDetails(string ownerGuid, DateTime startDate, DateTime endDate)
        {
            var completed = (int)TransactionStatus.Completed;

            var query = GetQuery(t => t.OwnerGuid == ownerGuid && t.TransferredDate.HasValue && t.TransferredDate >= startDate && t.TransferredDate < endDate && t.Status == completed);

            var detailQuery = (from t in query
                                      group t by new { t.PaymentType, t.MerchantId, IsWithdrawalPayment = t.WithdrawalId == null ? false : true } into gr
                                      select new
                                      {
                                          Amount = gr.Sum(p => p.TransactionAmount),
                                          Count = gr.Count(),
                                          MerchantId = gr.Key.MerchantId,
                                          PaymentType = gr.Key.PaymentType,
                                          IsWithdrawalPayment = gr.Key.IsWithdrawalPayment
                                      });

            var details = await detailQuery.AsNoTracking().ToListAsync();

            var items = new List<InvoiceDetail>();

            for (int i = 0; i < details.Count; i++)
            {
                var detail = details[i];

                var type = InvoiceDetailItemType.PardakhtPayDeposit;

                if(detail.PaymentType == (int)PaymentType.CardToCard)
                {
                    type = InvoiceDetailItemType.PardakhtPayDeposit;
                }
                else if(detail.PaymentType == (int)PaymentType.Mobile)
                {
                    if (detail.IsWithdrawalPayment)
                    {
                        type = InvoiceDetailItemType.PardakhtPalWithdrawal;
                    }
                    else
                    {
                        type = InvoiceDetailItemType.PardakhtPalDeposit;
                    }
                }
                else
                {
                    throw new Exception($"Payment type is not defined {detail.PaymentType}");
                }

                var item = new InvoiceDetail();
                item.MerchantId = detail.MerchantId;
                item.TotalAmount = detail.Amount;
                item.StartDate = startDate;
                item.EndDate = endDate;
                item.TotalCount = detail.Count;
                item.ItemTypeId = (int)type;

                items.Add(item);
            }

            return items;
        }

        public async Task<decimal> GetTotalPaymentAmountForPaymentGateway(MobileTransferCardAccount account, DateTime date)
        {
            var query = GetQuery();

            var completed = (int)TransactionStatus.Completed;

            return await (from t in query
                          where t.PaymentType == account.PaymentProviderType
                          && t.ProxyPaymentAccountId == account.Id
                          && t.Status == completed
                          && t.CreationDate >= date
                          select t.TransactionAmount).SumAsync();
        }
    }
}
