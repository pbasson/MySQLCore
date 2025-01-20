using MySQLCore.Core.Models.DTOs;
using FluentValidation;

namespace MySQLCore.Core.Models.Validators;

public class CRUDTransactionValidator : AbstractValidator<CRUDTransactionDTO>
{
    public CRUDTransactionValidator()
    {
        RuleFor(x => x.Id).NotEqual(0).WithMessage("Id is Invalid");
        RuleFor(x => x).NotNull().WithMessage("Model is Invalid");
    }
}
