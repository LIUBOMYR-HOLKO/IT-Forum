namespace IT_Forum.Helpers
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 100;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 100 : pageSize;
        }

        public void ToValidOptions(int countOfItems)
        {
            if (PageNumber * PageSize <= countOfItems + PageSize) return;
            PageNumber = countOfItems / PageSize;
            if (countOfItems % PageSize != 0)
                ++PageNumber;
        }
    }
}