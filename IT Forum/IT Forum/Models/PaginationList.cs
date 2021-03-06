using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace IT_Forum.Models

{
    public class PaginationList<T> : List<T>
    {
        public int PageIndex { get; private set; }

        public int TotalPages { get; private set; }


        public PaginationList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginationList<T>> CreateAsync(List<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count;

            var items = await source.Skip(() => (pageIndex - 1) * pageSize).Take(() => pageSize).ToListAsync();
            return new PaginationList<T>(items, count, pageIndex, pageSize);
        }

    }
}
