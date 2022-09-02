using Downcast.PasswordManager.Model.Input;

namespace Downcast.PasswordManager;

public interface IPasswordManager
{
    /// <summary>
    /// Changes the password for the given account.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task ChangeUserPassword(string userId, ChangePasswordRequest request);

    /// <summary>
    /// Returns a map of password requirements, where the key is the requirement name and the value is a regex
    /// that needs to pass for the password to be valid.
    /// </summary>
    /// <returns></returns>
    Dictionary<string, string> GetPasswordRequirements();
}