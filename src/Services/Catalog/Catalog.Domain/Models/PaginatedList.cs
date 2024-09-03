using Catalog.Domain.Models.Pagination;

namespace Catalog.Domain.Models
{
    public class PaginatedList<T>(IEnumerable<T> items, int totalCount, int pageSize)
        where T : class
    {
        public IEnumerable<T> Items { get; } = items;
        public int TotalPages { get; } = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
