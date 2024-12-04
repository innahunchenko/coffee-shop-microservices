using Catalog.Domain.Models.Pagination;

namespace Catalog.Domain.Models
{
    public class CachedContext
    {
        public string Index { get; set; }
        public string TotalKey { get; set; }
        public PaginationParameters PaginationParameters { get; set; }

        public CachedContext(string indexKeyTemplate, string filterKey, PaginationParameters paginationParameters)
        {
            Index = string.Format(indexKeyTemplate + ":page:{1}", filterKey.ToLower(), paginationParameters.PageNumber);
            TotalKey = $"{Index}:total";
            PaginationParameters = paginationParameters;
        }
    }
}
