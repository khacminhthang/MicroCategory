using MicroCategory.Domain.Common.Queries;
using MicroCategory.Domain.Dtos;

namespace MicroCategory.Domain.Queries
{
    /// <summary>
    /// GetTermByTermIdQuery
    /// </summary>
    public class GetTermWithTermMetaByTermIdQuery : IQuery<object>
    {
        /// <summary>
        /// field Id for input value
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Initialize a new instance of the <see cref="GetTermWithTermMetaByTermIdQuery"/> class
        /// </summary>
        /// <param name="Id"></param>
        public GetTermWithTermMetaByTermIdQuery(int Id)
        {
            this.Id = Id;
        }

    }
}
