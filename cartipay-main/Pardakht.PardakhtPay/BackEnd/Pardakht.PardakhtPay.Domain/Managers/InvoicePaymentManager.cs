using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Extensions;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class InvoicePaymentManager : BaseManager<InvoicePayment, IInvoicePaymentRepository>, IInvoicePaymentManager
    {
        IAesEncryptionService _AesEncryptionService = null;
        ITimeZoneService _TimeZoneService = null;

        public InvoicePaymentManager(IInvoicePaymentRepository repository,
            IAesEncryptionService aesEncryptionService,
            ITimeZoneService timeZoneService):base(repository)
        {
            _AesEncryptionService = aesEncryptionService;
            _TimeZoneService = timeZoneService;
        }

        public async Task<ListSearchResponse<IEnumerable<InvoicePaymentDTO>>> Search(InvoicePaymentSearchArgs args)
        {
            var query = Repository.GetQuery();

            var itemQuery = (from q in query
                             select new InvoicePaymentDTO()
                             {
                                 Id = q.Id,
                                 Amount = q.Amount,
                                 CreateDate = q.CreateDate,
                                 OwnerGuid = q.OwnerGuid,
                                 PaymentDate = q.PaymentDate,
                                 PaymentReference = q.PaymentReference,
                                 TenantGuid = q.TenantGuid
                             });

            if (!string.IsNullOrEmpty(args.OwnerGuid))
            {
                itemQuery = itemQuery.Where(t => t.OwnerGuid == args.OwnerGuid);
            }

            if (args.FilterModel != null)
            {
                itemQuery = itemQuery.ApplyParameters(args.FilterModel, _AesEncryptionService);
            }

            bool sort = false;

            if (!string.IsNullOrEmpty(args.SortColumn))
            {
                switch (args.SortColumn)
                {
                    case "id":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.Id);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.Id);
                        }
                        break;
                    case "paymentDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.PaymentDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.PaymentDate);
                        }
                        break;
                    case "createDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.CreateDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.CreateDate);
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
                    case "paymentReference":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.PaymentReference);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.PaymentReference);
                        }
                        break;
                }
            }

            if (!sort)
            {
                itemQuery = itemQuery.OrderByDescending(t => t.CreateDate);
            }

            var totalCount = itemQuery.Count();

            List<InvoicePaymentDTO> items = null;

            items = itemQuery.Skip(args.StartRow).Take(args.EndRow - args.StartRow).ToList();

            List<DateTime> createDates = items.Select(t => t.CreateDate).ToList();

            List<DateTime> paymentDates = items.Select(t => t.PaymentDate).ToList();

            string calendarCode = args.TimeZoneInfo.GetCalendarCode();

            var createConvertedDates = await _TimeZoneService.ConvertCalendarLocal(createDates, string.Empty, calendarCode);

            var paymentConvertedDates = await _TimeZoneService.ConvertCalendarLocal(paymentDates, string.Empty, calendarCode);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                item.CreateDateStr = createConvertedDates[i];
                item.PaymentDateStr = paymentConvertedDates[i];
            }

            return new ListSearchResponse<IEnumerable<InvoicePaymentDTO>>()
            {
                Items = items.AsEnumerable(),
                Success = true,
                Paging = new PagingHeader(totalCount, 0, 0, 0)
            };
        }
    }
}
