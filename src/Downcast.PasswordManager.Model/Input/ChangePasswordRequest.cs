using System.ComponentModel.DataAnnotations;

using Downcast.Common.Data.Validators.PasswordValidator;

namespace Downcast.PasswordManager.Model.Input;

public class ChangePasswordRequest : IValidatableObject
{
    [Password(false)]
    public string CurrentPassword { get; init; } = null!;

    [Password(true)]
    public string NewPassword { get; init; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CurrentPassword.Equals(NewPassword, StringComparison.Ordinal))
        {
            yield return new ValidationResult("New password must be different from old password.",
                                              new[] { nameof(NewPassword) });
        }
    }
}