using Bogus;

using Downcast.PasswordManager.Client.Model.Input;

namespace Downcast.PasswordManager.Tests.Utils.DataFakers;

public sealed class ChangePasswordRequestFaker : Faker<ChangePasswordRequest>
{
    private const string Password = "Password1234!";

    public ChangePasswordRequestFaker(bool equalPasswords = false)
    {
        RuleFor(auth => auth.CurrentPassword, Password);
        RuleFor(auth => auth.NewPassword, equalPasswords ? Password : Password + "1");
    }
}