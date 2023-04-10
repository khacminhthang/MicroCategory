
namespace MicroCategory.Domain.Dtos
{
    public class CTermDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public string? Type { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }
    }
}
