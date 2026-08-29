using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.DTOs.Pagination
{
    public class QueryParams : PaginationParams
    {
        public string? Search { get; set; }

        public string SortBy { get; set; } = "Id";

        public string SortDir { get; set; } = "asc";
    }
}
