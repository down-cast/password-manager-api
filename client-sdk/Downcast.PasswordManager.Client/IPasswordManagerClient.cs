using Downcast.PasswordManager.Client.Model.Input;

using Refit;

namespace Downcast.PasswordManager.Client;

public interface IPasswordManagerClient
{
    /// <summary>
    /// Changes logged in user's password.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Put("/api/v1/password")]
    Task<HttpResponseMessage> UpdatePassword([Body] ChangePasswordRequest request, [Authorize] string token);

    /// <summary>
    /// Gets the password requirements.
    /// </summary>
    /// <returns></returns>
    [Get("/api/v1/password/requirements")]
    Task<ApiResponse<IDictionary<string, object>>> GetPasswordRequirements();
}