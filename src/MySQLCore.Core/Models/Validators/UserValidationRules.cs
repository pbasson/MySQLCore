namespace MySQLCore.Core.Models.Validators;

public static class UserValidationRules
{
    public static IRuleBuilderOptions<T, string?> ValidUserName<T>( this IRuleBuilderInitial<T, string?> rule)
    {
        return rule.Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Username is required.")
            .MaximumLength(100).WithMessage("Username must be less than 100 characters.");
    }

    public static IRuleBuilderOptions<T, string?> ValidEmail<T>( this IRuleBuilderInitial<T, string?> rule)
    {
        return rule.Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Email is required.")
            .MaximumLength(100).WithMessage("Email must be less than 100 characters.")
            .EmailAddress().WithMessage("Email is invalid.");
    }

    public static IRuleBuilderOptions<T, string?> ValidName<T>( this IRuleBuilder<T, string?> rule, string propertyName)
    {
        return rule.NotEmpty().WithMessage($"{propertyName} is required.")
            .MaximumLength(100).WithMessage($"{propertyName} must be less than 100 characters.");
    }
}