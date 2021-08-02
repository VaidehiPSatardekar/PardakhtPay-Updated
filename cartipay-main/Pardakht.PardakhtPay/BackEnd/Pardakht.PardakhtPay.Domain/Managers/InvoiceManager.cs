using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class InvoiceManager : BaseManager<Invoice, IInvoiceRepository>, IInvoiceManager
    {
        IInvoiceDetailRepository _DetailRepository = null;
        IAesEncryptionService _AesEncryptionService = null;
        ITimeZoneService _TimeZoneService = null;

        public InvoiceManager(IInvoiceRepository repository,
            IInvoiceDetailRepository invoiceDetailRepository,
            IAesEncryptionService aesEncryptionService,
            ITimeZoneService timeZoneService) : base(repository)
        {
            _DetailRepository = invoiceDetailRepository;
            _AesEncryptionService = aesEncryptionService;
            _TimeZoneService = timeZoneService;
        }

        public async Task<Invoice> AddInvoice(Invoice invoice, List<InvoiceDetail> details)
        {
            invoice.Amount = details.Sum(t => t.Amount);
            invoice.CreateDate = DateTime.UtcNow;
            invoice.DueDate = invoice.EndDate.Date;

            var item = await AddAsync(invoice);
            await SaveAsync();

            for (int i = 0; i < details.Count; i++)
            {
                var detail = details[i];

                detail.CreateDate = DateTime.UtcNow;
                detail.InvoiceId = item.Id;
                await _DetailRepository.InsertAsync(detail);
            }

            await _DetailRepository.SaveChangesAsync();

            return item;
        }

        public async Task<Invoice> GetLastInvoiceAsync(string ownerGuid)
        {
            return await Repository.GetLastInvoiceAsync(ownerGuid);
        }

        public async Task<InvoiceDTO> GetItemById(int id)
        {
            var invoice = await GetEntityByIdAsync(id);

            var dto = AutoMapper.Mapper.Map<Invoice, InvoiceDTO>(invoice);

            dto.Items = new List<InvoiceDetailDTO>();

            var details = await _DetailRepository.GetItemsAsync(t => t.InvoiceId == id);

            for (int i = 0; i < details.Count; i++)
            {
                var detailDto = AutoMapper.Mapper.Map<InvoiceDetail, InvoiceDetailDTO>(details[i]);

                dto.Items.Add(detailDto);
            }

            return dto;
        }

        public async Task<ListSearchResponse<IEnumerable<InvoiceSearchDTO>>> Search(InvoiceSearchArgs args)
        {
            var query = Repository.GetQuery();

            var itemQuery = (from q in query
                             select new InvoiceSearchDTO()
                             {
                                 Id = q.Id,
                                 Amount = q.Amount,
                                 CreateDate = q.CreateDate,
                                 DueDate = q.DueDate,
                                 EndDate = q.EndDate,
                                 OwnerGuid = q.OwnerGuid,
                                 StartDate = q.StartDate,
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
                    case "startDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.StartDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.StartDate);
                        }
                        break;
                    case "endDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.EndDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.EndDate);
                        }
                        break;
                    case "dueDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.DueDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.DueDate);
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
                }
            }

            if (!sort)
            {
                itemQuery = itemQuery.OrderByDescending(t => t.EndDate);
            }

            var totalCount = itemQuery.Count();

            List<InvoiceSearchDTO> items = null;

            items = itemQuery.Skip(args.StartRow).Take(args.EndRow - args.StartRow).ToList();

            List<DateTime> startDates = items.Select(t => t.StartDate).ToList();

            List<DateTime> endDates = items.Select(t => t.EndDate).ToList();

            string calendarCode = args.TimeZoneInfo.GetCalendarCode();

            var startConvertedDates = await _TimeZoneService.ConvertCalendarLocal(startDates, string.Empty, calendarCode);

            var endConvertedDates = await _TimeZoneService.ConvertCalendarLocal(endDates, string.Empty, calendarCode);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                item.StartDateStr = startConvertedDates[i];
                item.EndDateStr = endConvertedDates[i];
            }

            return new ListSearchResponse<IEnumerable<InvoiceSearchDTO>>()
            {
                Items = items.AsEnumerable(),
                Success = true,
                Paging = new PagingHeader(totalCount, 0, 0, 0)
            };
        }
    }
}
