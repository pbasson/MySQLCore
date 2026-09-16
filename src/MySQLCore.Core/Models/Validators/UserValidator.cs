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

public class CreateUserValidator : AbstractValidator<CreateUserDTO>
{
    public CreateUserValidator(IUserRepo repo)
    {
        RuleFor(x => x.UserName).Cascade(CascadeMode.Stop).NotEmpty().MaximumLength(100).WithMessage("Username must be less than 100 characters ")
            .MustAsync(async (username, cancellationToken) => {
                var getUsername = await repo.GetUsernameAsync(username!, cancellationToken);
                return getUsername == null || getUsername.Id <= 0; 
            }).WithMessage("Username is required.");

        RuleFor(x => x.Email).Cascade(CascadeMode.Stop).NotEmpty().MaximumLength(100).WithMessage("Email must be less than 100 characters").EmailAddress()
            .MustAsync(async (email, cancellationToken) =>
                !await repo.CheckEmailAsync(email, cancellationToken)
            ).WithMessage("Email already exists.");
    }
}