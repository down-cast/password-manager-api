using Downcast.Common.Data.Validators.PasswordValidator;
using Downcast.Common.Errors;
using Downcast.PasswordManager.Model.Input;
using Downcast.UserManager.Client;
using Downcast.UserManager.Client.Model;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Refit;

namespace Downcast.PasswordManager;

public class PasswordManager : IPasswordManager
{
    private readonly IUserManagerClient _userManagerClient;
    private readonly ILogger<PasswordManager> _logger;
    private readonly IOptions<PasswordRequirementsOptions> _options;

    public PasswordManager(
        IUserManagerClient userManagerClient,
        ILogger<PasswordManager> logger,
        IOptions<PasswordRequirementsOptions> options)
    {
        EnsurePasswordRequirementsAreSet(options);
        _userManagerClient = userManagerClient;
        _logger = logger;
        _options = options;
    }

    private static void EnsurePasswordRequirementsAreSet(IOptions<PasswordRequirementsOptions> options)
    {
        if (options is not { Value.Requirements.Count: > 0 })
        {
            throw new DcException(ErrorCodes.InternalServerError, "Password requirements are not configured");
        }
    }

    public async Task ChangeUserPassword(string userId, ChangePasswordRequest request)
    {
        User user = await GetUserById(userId).ConfigureAwait(false);
        bool validCredentials = await CredentialsAreValid(user.Email, request.CurrentPassword).ConfigureAwait(false);
        if (!validCredentials)
        {
            throw new DcException(ErrorCodes.InvalidCredentials, "Invalid credentials");
        }

        await UpdatePassword(userId, request.NewPassword).ConfigureAwait(false);
    }

    public Dictionary<string, string> GetPasswordRequirements() => _options.Value.Requirements;

    private async Task UpdatePassword(string userId, string password)
    {
        HttpResponseMessage response = await _userManagerClient.UpdatePassword(userId, new UpdatePasswordInput
        {
            Password = password
        }).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    private async Task<User> GetUserById(string userId)
    {
        ApiResponse<User> userResponse = await _userManagerClient.GetUser(userId).ConfigureAwait(false);
        userResponse = await userResponse.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
        return userResponse.Content!;
    }

    private async Task<bool> CredentialsAreValid(string email, string password)
    {
        HttpResponseMessage response = await _userManagerClient.ValidateCredentials(new AuthenticationRequest
        {
            Password = password,
            Email = email
        }).ConfigureAwait(false);

        return response.IsSuccessStatusCode;
    }
}