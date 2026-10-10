
using Microsoft.AspNetCore.Identity;
using txtOnly.Infrastructure.Identity;

namespace txtOnly.Infrastructure.Validators;

public sealed class UserValidator : IUserValidator<ApplicationUser>
{
    public async Task<IdentityResult> ValidateAsync(
          UserManager<ApplicationUser> manager,
          ApplicationUser user)
    {
        var errors = new List<IdentityError>();
        var userName = await manager.GetUserNameAsync(user);

        if (string.IsNullOrWhiteSpace(userName))
        {
            errors.Add(new IdentityError
            {
                Code = "InvalidUserName",
                Description = "User name cannot be null or empty."
            });
        }
        else if (manager.Options.User.AllowedUserNameCharacters is not null &&
                 userName.Any(character => !manager.Options.User.AllowedUserNameCharacters.Contains(character)))
        {
            errors.Add(new IdentityError
            {
                Code = "InvalidUserName",
                Description = $"User name '{userName}' is invalid, can only contain letters or digits."
            });
        }

        if (manager.Options.User.RequireUniqueEmail && !string.IsNullOrWhiteSpace(user.Email))
        {
            var existingUser = await manager.FindByEmailAsync(user.Email);
            if (existingUser is not null && existingUser.Id != user.Id)
            {
                errors.Add(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = $"Email '{user.Email}' is already taken."
                });
            }
        }

        return errors.Count == 0
            ? IdentityResult.Success
            : IdentityResult.Failed(errors.ToArray());
    }
}
