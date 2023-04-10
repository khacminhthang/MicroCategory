using MicroCategory.Domain.Validations.Term;

namespace MicroCategory.Domain.Commands
{
    public sealed class CreateTermCommand : TermCommand
    {
        public override bool IsValid()
        {
            ValidationResult = new CreateTermCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
