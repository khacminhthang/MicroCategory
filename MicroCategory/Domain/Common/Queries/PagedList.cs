using Newtonsoft.Json;

namespace MicroCategory.Domain.Common.Queries
{
    public class PagedList<T>
    {
        [JsonProperty("TotalItems")]
        public int TotalItems { get; }
        [JsonProperty("PageNumber")]
        public int PageNumber { get; }
        [JsonProperty("PageSize")]
        public int PageSize { get; }
        [JsonProperty("List")]
        public List<T> List { get; }

        //public PagedList() { }

        [JsonConstructor]
        public PagedList(IEnumerable<T> source, int totalRecord, int pageNumber, int pageSize)
        {
            TotalItems = totalRecord;
            PageNumber = pageNumber;
            PageSize = pageSize;
            List = source.ToList();
        }

        //[JsonConstructor]
        public PagedList(IQueryable<T> source, int totalRecord, int pageNumber, int pageSize)
        {
            TotalItems = totalRecord;
            PageNumber = pageNumber;
            PageSize = pageSize;
            List = source.ToList();
        }

        [JsonConstructor]
        public PagedList(List<T> list, int totalItems, int pageNumber, int pageSize)
        {
            this.TotalItems = totalItems;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.List = list;
        }

        public int TotalPages => (int)Math.Ceiling(this.TotalItems / (double)this.PageSize);

        public PagingHeader GetHeader()
        {
            return new PagingHeader(this.TotalItems, this.PageNumber, this.PageSize, this.TotalPages);
        }
    }
}
