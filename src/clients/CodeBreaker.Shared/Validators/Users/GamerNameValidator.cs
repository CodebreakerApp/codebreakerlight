using FluentValidation;

namespace CodeBreaker.Shared.Validators.Users;

public class GamerNameValidator : AbstractValidator<string>
{
    public GamerNameValidator(string name = "GamerName")
    {
        RuleFor(_ => _)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50)
            .WithName(name);
    }
}
