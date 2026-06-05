namespace MySQLCore.Core.Models.Validators;

public class UserValidator : AbstractValidator<UserDTO>
{
    public UserValidator()
    {
        RuleFor(x => x.Id).NotEqual(0).WithMessage("Id is Invalid");
        RuleFor(x => x).NotNull().WithMessage("Model is Invalid");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is Required").MaximumLength(100).WithMessage("Username must be less than 100 characters ");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is Required").MaximumLength(100).WithMessage("FirstName must be less than 100 characters ");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is Required").MaximumLength(100).WithMessage("LastName must be less than 100 characters ");
    }
}
