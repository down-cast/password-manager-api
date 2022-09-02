using Downcast.PasswordManager.Model.Input;
using Downcast.SessionManager.SDK.Authentication.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Downcast.PasswordManager.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/password")]
public class PasswordManagerController : ControllerBase
{
    private readonly IPasswordManager _passwordManager;

    public PasswordManagerController(IPasswordManager passwordManager)
    {
        _passwordManager = passwordManager;
    }

    /// <summary>
    /// Updates the password for the given user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task UpdatePassword(ChangePasswordRequest request)
    {
        return _passwordManager.ChangeUserPassword(User.UserId(), request);
    }


    /// <summary>
    /// Returns password requirements
    /// </summary>
    /// <returns></returns>
    [HttpGet("requirements")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Dictionary<string, string> GetPasswordRequirements()
    {
        return _passwordManager.GetPasswordRequirements();
    }
}