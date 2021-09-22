
using System;

namespace PersonProfile_DF.Api.Models
{
    public class QueryStringParameters
    {
        const int maxPageSize = 50;
        private int _pageSize = 10;

        public string RequestedFields { get; set; }

        // Use to sort multiple columns; e.g "PersonName asc, PersonEmail desc"
        public string SortColumns { get; set; }
        
        // Use to specify the pageNumber to fetch
        public int PageNumber { get; set; } = 1;
        
        // Use to specify the number-of-records on each page
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }

    public class Pagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? TotalRecords { get; set; }
        public int? TotalPages => TotalRecords.HasValue ? (int)Math.Ceiling(TotalRecords.Value / (double)PageSize) : (int?)null;
    }
}

