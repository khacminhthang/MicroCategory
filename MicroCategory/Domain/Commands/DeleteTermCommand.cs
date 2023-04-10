
using MicroCategory.Domain.Validations.Term;

namespace MicroCategory.Domain.Commands
{
    public sealed class DeleteTermCommand : TermCommand
    {
        public override bool IsValid()
        {
            ValidationResult = new DeleteTermCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
