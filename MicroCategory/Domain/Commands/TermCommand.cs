using MicroCategory.Domain.Common.Commands;

namespace MicroCategory.Domain.Commands
{
    public abstract class TermCommand : ICommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid Slug { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public IList<TermMeta> TermMetas { get; set; }

    }
    public class TermMeta
    {
        public long TermId { get; set; }
        public string MetaKey { get; set; }
        public string MetaValue { get; set; }
    }
}
