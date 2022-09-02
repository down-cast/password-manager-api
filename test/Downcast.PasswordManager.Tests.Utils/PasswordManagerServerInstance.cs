using System.Net;
using System.Text;

using Downcast.PasswordManager.API.Controllers;
using Downcast.SessionManager.SDK.Client;
using Downcast.SessionManager.SDK.Client.Model;
using Downcast.UserManager.Client;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Moq;

using Refit;

namespace Downcast.PasswordManager.Tests.Utils;

public class PasswordManagerServerInstance : WebApplicationFactory<PasswordManagerController>
{
    private readonly JsonWebTokenHandler _handler = new();
    private readonly Mock<ISessionManagerClient> _sessionManagerMock;
    private readonly Mock<IUserManagerClient> _userManagerClient;

    public PasswordManagerServerInstance(
        Mock<IUserManagerClient> userManagerClient,
        Mock<ISessionManagerClient> sessionManagerMock)
    {
        _userManagerClient = userManagerClient;
        _sessionManagerMock = sessionManagerMock;
        SetupTokenValidation();
    }

    private void SetupTokenValidation()
    {
        _sessionManagerMock.Setup(client => client.ValidateSessionToken(It.IsAny<string>()))
            .Returns<string>(token =>
            {
                try
                {
                    _handler.ReadJsonWebToken(token);
                    return Task.CompletedTask;
                }
                catch (Exception)
                {
                    throw ApiException.Create(
                            new HttpRequestMessage(HttpMethod.Post, "http://localhost"),
                            HttpMethod.Post,
                            new HttpResponseMessage(HttpStatusCode.Unauthorized), new RefitSettings())
                        .Result;
                }
            });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(_sessionManagerMock.Object);
            services.AddSingleton(_userManagerClient.Object);
        });
    }
}