namespace MicroCategory.Domain.Common.Queries
{
    public interface IPagedQuery : IQuery
    {
        int PageNumber { get; }

        int PageSize { get; }

        string OrderColumn { get; }

        bool OrderValue { get; }
    }
}
