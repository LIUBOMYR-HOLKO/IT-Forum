using System;
using System.Collections.Generic;
using IT_Forum.Responses;

namespace IT_Forum.Helpers
{
    public class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedResponse<T>(List<T> pagedData, PaginationFilter validFilter,
            int totalRecords)
        {
            var response = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
            var totalPages = totalRecords / (double)validFilter.PageSize;
            var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            response.TotalPages = roundedTotalPages;
            response.PageSize = validFilter.PageSize == new PaginationFilter().PageSize ? totalRecords : validFilter.PageSize;
            response.TotalRecords = totalRecords;
            return response;
        }
    }
}