using System.Text;

using Bogus;

using Downcast.PasswordManager.Client;
using Downcast.SessionManager.SDK.Authentication.Extensions;
using Downcast.SessionManager.SDK.Client;
using Downcast.UserManager.Client;
using Downcast.UserManager.Client.Model;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Moq;

using Refit;

namespace Downcast.PasswordManager.Tests.Utils;

public class BaseTestClass
{
    protected Faker Faker { get; } = new();
    protected Mock<ISessionManagerClient> SessionManagerMock { get; } = new(MockBehavior.Strict);
    protected Mock<IUserManagerClient> UserManagerMock { get; } = new(MockBehavior.Strict);
    protected IPasswordManagerClient Client { get; }
    private readonly JsonWebTokenHandler _handler = new();
    private readonly SecurityKey _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is a test key"));

    public BaseTestClass()
    {
        HttpClient httpClient = new PasswordManagerServerInstance(UserManagerMock, SessionManagerMock).CreateClient();
        Client = RestService.For<IPasswordManagerClient>(httpClient);
    }

    protected string GenerateValidToken(User user)
    {
        return _handler.CreateToken(new SecurityTokenDescriptor
        {
            Expires = DateTime.MaxValue,
            Claims = new Dictionary<string, object>
            {
                { ClaimNames.Role, user.Roles },
                { ClaimNames.Email, user.Email },
                { ClaimNames.DisplayName, user.DisplayName ?? "" },
                { ClaimNames.UserId, user.Id },
                { ClaimNames.ProfilePictureUri, user.ProfilePictureUri?.ToString() ?? "" },
            },
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature),
            Issuer = "test",
            IssuedAt = DateTime.UtcNow,
            Audience = "test"
        });
    }
}