namespace MySQLCore.Core.Validators;

public class UserValidator : AbstractValidator<CreateUserDTO>
{
    public UserValidator(IUserRepo repo)
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress()
        .WithMessage("Valid email is required.")
        .MustAsync(async (email, cancellationToken) =>
        {
            return await repo.CheckEmailAsync(email, cancellationToken);
        })
        .WithMessage("Email already exists.");

    }
}