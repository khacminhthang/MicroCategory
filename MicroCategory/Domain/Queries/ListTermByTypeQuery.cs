using MicroCategory.Domain.Common.Queries;
using MicroCategory.Domain.Dtos;
using System.ComponentModel.DataAnnotations;

namespace MicroCategory.Domain.Queries
{
    /// <summary>
    /// ListTermByTypeQuery
    /// </summary>
    public class ListTermByTypeQuery : IQuery<PagedList<CTermDto>>
    {
        /// <summary>
        /// PageNumber
        /// </summary>
        [Required(ErrorMessage = "Phải nhập số trang!")]
        public int PageNumber { get; set; }

        /// <summary>
        /// PageSize
        /// </summary>
        public int PageSize { get; set; }

        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
