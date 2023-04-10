using FluentValidation;
using MicroCategory.Domain.Commands;

namespace MicroCategory.Domain.Validations.Term
{
    public class CreateTermCommandValidator : AbstractValidator<CreateTermCommand>
    {
        public CreateTermCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
