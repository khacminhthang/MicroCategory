using MicroCategory.Domain.Common.Queries;
using MicroCategory.Domain.Dtos;

namespace MicroCategory.Domain.Queries
{
    /// <summary>
    /// GetDataByParentIdQuery
    /// </summary>
    public class GetDataByParentIdQuery : IQuery<IList<CTermDto>>
    {
        /// <summary>
        /// field Id for input value
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Initialize a new instance of the <see cref="GetDataByParentIdQuery"/> class
        /// </summary>
        /// <param name="Id"></param>
        public GetDataByParentIdQuery(int Id)
        {
            this.Id = Id;
        }

    }
}
