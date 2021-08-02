using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class ListSearchArgs
    {
        public string Filter { get; set; }

        public int PageSize { get; set; } = 0;

        public int PageNumber { get; set; } = 1;

        public string SortColumn { get; set; }

        public string SortOrder { get; set; }

        public string TimeZoneInfoId { get; set; }

        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(TimeZoneInfoId))
                {
                    return System.TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfoId);
                }

                return TimeZoneInfo.Utc;
            }
        }
    }

    public class AgGridSearchArgs
    {
        public int StartRow { get; set; }

        public int EndRow { get; set; }

        public string SortColumn { get; set; }

        public string SortOrder { get; set; }

        public string TimeZoneInfoId { get; set; }

        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(TimeZoneInfoId))
                {
                    return System.TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfoId);
                }

                return TimeZoneInfo.Utc;
            }
        }
    }

    public class ListSearchResponse<T> : WebResponse where T : IEnumerable
    {
        public T Items { get; set; }

        public LinkInfo Links { get; set; }

        public PagingHeader Paging { get; set; }
    }

    public class LinkInfo
    {
        public string Href { get; set; }

        public string Rel { get; set; }

        public string Method { get; set; }
    }

    public class PagingHeader
    {
        public int TotalItems { get; }

        public int PageNumber { get; }

        public int PageSize { get; }

        public int TotalPages { get; }

        public PagingHeader(int totalItems, int pageNumber, int pageSize, int totalPages)
        {
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
        }
    }

    public class SortedPagedList<T>
    {
        public SortedPagedList(List<T> source, int pageNumber, int pageSize, string sortColumn, string sortOrder)
        {
            TotalItems = source.Count();
            PageNumber = pageNumber;
            PageSize = pageSize;

            List = source.ToList();

            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                //var prop = typeof(T).GetProperty(sortColumn); //Models in Angular UI side are camel case so need to be translated to Pascal case 
                var prop = typeof(T).GetProperty(sortColumn.Length == 1 ? sortColumn.ToUpper() : char.ToUpper(sortColumn[0]) + sortColumn.Substring(1));
                if (!string.IsNullOrWhiteSpace(sortOrder))
                {
                    if (sortOrder.ToLower().StartsWith("d"))
                    {
                        List = List.OrderByDescending(c => prop.GetValue(c, null)).ToList();

                    }
                    else
                    {
                        List = List.OrderBy(c => prop.GetValue(c, null)).ToList();
                    }
                }
                else
                {
                    List = List.OrderBy(c => prop.GetValue(c, null)).ToList();
                }
            }
            if (this.PageSize != 0)
            {
                List = List
                        .Skip(pageSize * (pageNumber - 1))
                        .Take(pageSize)
                        .ToList();
            }
        }

        public int TotalItems { get; }

        public int PageNumber { get; }

        public int PageSize { get; }

        public List<T> List { get; }

        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public int NextPageNumber => HasNextPage ? PageNumber + 1 : TotalPages;

        public int PreviousPageNumber => HasPreviousPage ? PageNumber - 1 : 1;

        public PagingHeader GetHeader()
        {
            return new PagingHeader(TotalItems, PageNumber, PageSize, TotalPages);
        }
    }

    public class AgGridFilterTypeModel
    {
        public string FilterType { get; set; }
    }

    public abstract class AgGridFilterBase
    {
        public string Type { get; set; }

        public abstract List<AgGridFilter> GetFilters();
    }

    public class AgGridFilter
    {
        public object Value { get; set; }

        public string Type { get; set; }
    }

    public class AgGridNumberFilter : AgGridFilterBase
    {
        public decimal? Filter { get; set; }

        public decimal? FilterTo { get; set; }

        public override List<AgGridFilter> GetFilters()
        {
            var list = new List<AgGridFilter>();

            if (Type == Helper.AgGridTypes.InRange)
            {
                if (Filter.HasValue)
                {
                    list.Add(new AgGridFilter() { Value = Filter.Value, Type = Helper.AgGridTypes.GreaterThanOrEqual });
                }

                if (FilterTo.HasValue)
                {
                    list.Add(new AgGridFilter() { Value = FilterTo.Value, Type = Helper.AgGridTypes.LessThanOrEqual });
                }
            }
            else
            {
                if (Filter.HasValue)
                {
                    list.Add(new AgGridFilter() { Value = Filter.Value, Type = Type });
                }
            }

            return list;
        }
    }

    public class AgGridIntegerFilter : AgGridFilterBase
    {
        public int? Filter { get; set; }

        public int? FilterTo { get; set; }

        public override List<AgGridFilter> GetFilters()
        {
            var list = new List<AgGridFilter>();

            if (Type == Helper.AgGridTypes.InRange)
            {
                if (Filter.HasValue)
                {
                    list.Add(new AgGridFilter() { Value = Filter.Value, Type = Helper.AgGridTypes.GreaterThanOrEqual });
                }

                if (FilterTo.HasValue)
                {
                    list.Add(new AgGridFilter() { Value = FilterTo.Value, Type = Helper.AgGridTypes.LessThanOrEqual });
                }
            }
            else
            {
                if (Filter.HasValue)
                {
                    list.Add(new AgGridFilter() { Value = Filter.Value, Type = Type });
                }
            }

            return list;
        }
    }

    public class AgGridDateFilter : AgGridFilterBase
    {
        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public override List<AgGridFilter> GetFilters()
        {
            DateTime? filterFrom = null;
            DateTime? filterTo = null;

            if (!string.IsNullOrEmpty(DateFrom))
            {
                if(DateTime.TryParseExact(DateFrom, Helper.AgGridDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime from))
                {
                    filterFrom = from;
                }
            }

            if (!string.IsNullOrEmpty(DateTo))
            {
                if(DateTime.TryParseExact(DateTo, Helper.AgGridDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime to))
                {
                    filterTo = to;
                }
            }

            var list = new List<AgGridFilter>();

            if (Type == Helper.AgGridTypes.InRange)
            {
                if (filterFrom.HasValue)
                {
                    list.Add(new AgGridFilter() { Value = filterFrom.Value, Type = Helper.AgGridTypes.GreaterThanOrEqual });
                }

                if (filterTo.HasValue)
                {
                    list.Add(new AgGridFilter() { Value = filterTo.Value, Type = Helper.AgGridTypes.LessThanOrEqual });
                }
            }
            else
            {
                if (filterFrom.HasValue)
                {
                    //list.Add(new AgGridFilter() { Value = Filter.Value, Type = Type });

                    list.Add(new AgGridFilter() { Value = filterFrom.Value, Type = Helper.AgGridTypes.GreaterThanOrEqual });

                    list.Add(new AgGridFilter() { Value = filterFrom.Value.AddDays(1), Type = Helper.AgGridTypes.LessThan });
                }
            }

            return list;
        }
    }

    public class AgGridNullableDateFilter : AgGridFilterBase
    {
        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public override List<AgGridFilter> GetFilters()
        {
            DateTime? filterFrom = null;
            DateTime? filterTo = null;

            if (!string.IsNullOrEmpty(DateFrom))
            {
                if (DateTime.TryParseExact(DateFrom, Helper.AgGridDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime from))
                {
                    filterFrom = (DateTime?)from;
                }
            }

            if (!string.IsNullOrEmpty(DateTo))
            {
                if (DateTime.TryParseExact(DateTo, Helper.AgGridDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime to))
                {
                    filterTo = (DateTime?)to;
                }
            }

            var list = new List<AgGridFilter>();

            if (Type == Helper.AgGridTypes.InRange)
            {
                if (filterFrom.HasValue)
                {
                    list.Add(new AgGridFilter() { Value = (DateTime?)filterFrom, Type = Helper.AgGridTypes.GreaterThanOrEqual });
                }

                if (filterTo.HasValue)
                {
                    list.Add(new AgGridFilter() { Value = (DateTime?)filterTo, Type = Helper.AgGridTypes.LessThanOrEqual });
                }
            }
            else
            {
                if (filterFrom.HasValue)
                {
                    //list.Add(new AgGridFilter() { Value = Filter.Value, Type = Type });

                    list.Add(new AgGridFilter() { Value = (DateTime?)filterFrom, Type = Helper.AgGridTypes.GreaterThanOrEqual });

                    list.Add(new AgGridFilter() { Value = (DateTime?)filterFrom.Value.AddDays(1), Type = Helper.AgGridTypes.LessThan });
                }
            }

            return list;
        }
    }

    public class AgGridTextFilter : AgGridFilterBase
    {
        public string Filter { get; set; }

        public override List<AgGridFilter> GetFilters()
        {
            if (string.IsNullOrEmpty(Filter))
            {
                return null;
            }

            var list = new List<AgGridFilter>();

            if (!string.IsNullOrEmpty(Filter))
            {
                list.Add(new AgGridFilter() { Value = Filter, Type = Type });
            }

            return list;
        }
    }
}
