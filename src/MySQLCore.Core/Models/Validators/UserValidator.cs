namespace MySQLCore.Core.Models.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDTO>
{
    public CreateUserValidator(IUserRepo repo)
    {
        RuleFor(x => x.UserName).ValidUserName()
            .MustAsync(async (username, cancellationToken) => {
                var getUsername = await repo.GetUsernameAsync(username!, cancellationToken);
                return getUsername == null || getUsername.Id <= 0; 
            }).WithMessage("Username is required.");

        RuleFor(x => x.Email).ValidEmail()
            .MustAsync(async (email, cancellationToken) =>
                !await repo.CheckEmailAsync(email!, cancellationToken)
            ).WithMessage("Email already exists.");

        RuleFor(x => x.FirstName).ValidName("FirstName");
        RuleFor(x => x.LastName).ValidName("LastName");
    }
}

public class UpdateUserValidator : AbstractValidator<UpdateUserDTO>
{
    public UpdateUserValidator(IUserRepo repo)
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id is invalid.");

        RuleFor(x => x.UserName).ValidUserName();

        RuleFor(x => x.Email).ValidEmail();

        RuleFor(x => x.FirstName).ValidName("FirstName");

        RuleFor(x => x.LastName).ValidName("LastName");

        RuleFor(x => x.UserName).ValidUserName()
            .MustAsync(async (username, cancellationToken) => {
                var getUsername = await repo.GetUsernameAsync(username!, cancellationToken);
                return getUsername == null || getUsername.Id <= 0; 
            }).WithMessage("Username already exists.");

        RuleFor(x => x.Email).ValidEmail()
            .MustAsync(async (email, cancellationToken) =>
                !await repo.CheckEmailAsync(email!, cancellationToken)
            ).WithMessage("Email already exists.");
    }
}