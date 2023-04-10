using MediatR;

namespace MicroCategory.Domain.Common.Queries
{
    public interface IQuery<TResult> : IRequest<TResult> where TResult : class { }

    public interface IQuery { }
}
