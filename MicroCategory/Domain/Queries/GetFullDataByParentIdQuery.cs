using MicroCategory.Domain.Common.Queries;
using MicroCategory.Domain.Dtos;

namespace MicroCategory.Domain.Queries
{
    /// <summary>
    /// GetFullDataByParentIdQuery
    /// </summary>
    public class GetFullDataByParentIdQuery : IQuery<IList<CTermWithTermMetaDto>>
    {
        /// <summary>
        /// field Id for input value
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Initialize a new instance of the <see cref="GetFullDataByParentIdQuery"/> class
        /// </summary>
        /// <param name="Id"></param>
        public GetFullDataByParentIdQuery(int Id)
        {
            this.Id = Id;
        }

    }
}
