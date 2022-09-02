namespace Downcast.PasswordManager.Client.Model.Input;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = null!;

    public string NewPassword { get; init; } = null!;
}