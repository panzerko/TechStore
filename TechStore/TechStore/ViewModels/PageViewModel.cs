using System;

namespace TechStore.ViewModels
{
    public class PageViewModel
    {
        public const int PageSize = 6;
        public int PageNumber { get; }
        public int TotalPages { get; }

        public PageViewModel(int count, int pageNumber)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
        }

        public bool HasPreviousPage => (PageNumber > 1);

        public bool HasNextPage => (PageNumber < TotalPages);
    }
}
