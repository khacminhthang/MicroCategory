using MediatR;

namespace MicroCategory.Infrastructure.Queries
{
    public interface IQuery<TResult> : IRequest<TResult> where TResult : class { }

    public interface IQuery { }
}
