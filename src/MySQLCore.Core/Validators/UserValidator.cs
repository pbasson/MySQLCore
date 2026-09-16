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

public class CreateUserValidator : AbstractValidator<CreateUserDTO>
{
    public CreateUserValidator(IUserRepo repo)
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required.");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (email, cancellationToken) =>
                !await repo.CheckEmailAsync(email, cancellationToken))
            .WithMessage("Email already exists.");
    }
}