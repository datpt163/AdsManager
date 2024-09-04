namespace FBAdsManager.Common.Paging
{
    public class PagingResponse
    {
        public PagingResponse(int totalCount, int pageIndex, int pageSize)
        {
            TotalCount = totalCount;
            CurrentPage = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        }

        public int CurrentPage { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
    }
}
