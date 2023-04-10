
using MicroCategory.Domain.Models;

namespace MicroCategory.Domain.Dtos
{
    public class CTermWithTermMetaDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Type { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public IList<CTermmetaDto> ListTermMeta { get; set; }
    }

}
