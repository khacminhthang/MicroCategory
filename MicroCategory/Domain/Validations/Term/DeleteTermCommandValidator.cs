using FluentValidation;
using MicroCategory.Domain.Commands;

namespace MicroCategory.Domain.Validations.Term
{
    public class DeleteTermCommandValidator : AbstractValidator<DeleteTermCommand>
    {
        public DeleteTermCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
