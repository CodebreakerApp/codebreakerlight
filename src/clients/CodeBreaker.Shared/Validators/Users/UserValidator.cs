using CodeBreaker.Shared.Models.Users;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CodeBreaker.Shared.Validators.Users;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator(IStringLocalizer<UserValidator> localizer)
    {
        RuleFor(_ => _.Email)
            .EmailAddress()
            .MaximumLength(100)
            .WithName(localizer["Email"]);
        RuleFor(_ => _.GivenName)
            .MinimumLength(2)
            .MaximumLength(50)
            .WithName(localizer["GivenName"]);
        RuleFor(_ => _.Surname)
            .MinimumLength(2)
            .MaximumLength(50)
            .WithName(localizer["Surname"]);
        RuleFor(_ => _.GamerName)
            .SetValidator(new GamerNameValidator(localizer["GamerName"]));
    }
}
