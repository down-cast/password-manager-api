using System.Net;

using Downcast.PasswordManager.Client.Model.Input;
using Downcast.PasswordManager.Tests.Utils;
using Downcast.PasswordManager.Tests.Utils.DataFakers;
using Downcast.UserManager.Client.Model;

using FluentAssertions;

using Moq;

using Refit;

namespace Downcast.PasswordManager.Tests;

public class PasswordManagerTests : BaseTestClass
{
    [Fact]
    public async Task UpdatePassword_Same_Password_Returns_Bad_Request()
    {
        ChangePasswordRequest changePasswordRequest = new ChangePasswordRequestFaker(true).Generate();
        // ensure passwords are equal
        changePasswordRequest.CurrentPassword.Should().Be(changePasswordRequest.NewPassword);

        // generate user and token
        User user = new UserFaker().Generate();
        string token = GenerateValidToken(user);

        // ensure that updating password returns Bad Request
        HttpResponseMessage response = await Client.UpdatePassword(changePasswordRequest, token).ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePassword_Weak_Password_Returns_Bad_Request()
    {
        // generate user and token
        User user = new UserFaker().Generate();
        string token = GenerateValidToken(user);

        // ensure that updating password returns Bad Request
        var weakPasswordRequest = new ChangePasswordRequest { CurrentPassword = "Password1234!", NewPassword = "1" };
        
        HttpResponseMessage response = await Client
            .UpdatePassword(weakPasswordRequest, token)
            .ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePassword_Returns_Unauthorized_With_Invalid_Token()
    {
        ChangePasswordRequest changePasswordRequest = new ChangePasswordRequestFaker().Generate();
        // generate user and token
        User user = new UserFaker().Generate();
        string invalidToken = "invalid" + GenerateValidToken(user);

        // ensure that updating password returns Bad Request
        HttpResponseMessage response =
            await Client.UpdatePassword(changePasswordRequest, invalidToken).ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task UpdatePassword_Success()
    {
        ChangePasswordRequest changePasswordRequest = new ChangePasswordRequestFaker().Generate();

        // generate user and token
        User user = new UserFaker().Generate();
        string token = GenerateValidToken(user);

        // setup user manager
        SetupGetUserToSucceed(user);
        SetupValidateCredentials(user, changePasswordRequest.CurrentPassword, HttpStatusCode.OK);
        SetupUpdatePasswordCall(user, changePasswordRequest.NewPassword, HttpStatusCode.OK);

        // Act
        HttpResponseMessage response =
            await Client.UpdatePassword(changePasswordRequest, token).ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task UpdatePassword_Invalid_Current_Password()
    {
        ChangePasswordRequest changePasswordRequest = new ChangePasswordRequestFaker().Generate();

        // generate user and token
        User user = new UserFaker().Generate();
        string token = GenerateValidToken(user);

        // setup user manager
        SetupGetUserToSucceed(user);
        SetupValidateCredentials(user, changePasswordRequest.CurrentPassword, HttpStatusCode.Unauthorized);
        SetupUpdatePasswordCall(user, changePasswordRequest.NewPassword, HttpStatusCode.OK);

        // Act
        HttpResponseMessage response =
            await Client.UpdatePassword(changePasswordRequest, token).ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    
    [Fact]
    public async Task GetPasswordRequirements_Returns_OK()
    {
        // Act
        ApiResponse<IDictionary<string, object>> response = await Client.GetPasswordRequirements().ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeEmpty();
    }

    private void SetupUpdatePasswordCall(User user, string password, HttpStatusCode statusCode)
    {
        UserManagerMock
            .Setup(client => client.UpdatePassword(
                       user.Id,
                       It.Is<UpdatePasswordInput>(input => input.Password.Equals(password))))
            .ReturnsAsync(new HttpResponseMessage(statusCode));
    }

    private void SetupValidateCredentials(User user, string password, HttpStatusCode responseStatusCode)
    {
        UserManagerMock.Setup(client => client.ValidateCredentials(
                                  It.Is<AuthenticationRequest>(
                                      req => req.Email.Equals(user.Email) &&
                                             req.Password.Equals(password))))
            .ReturnsAsync(new HttpResponseMessage(responseStatusCode));
    }

    private void SetupGetUserToSucceed(User user)
    {
        UserManagerMock.Setup(client => client.GetUser(user.Id))
            .ReturnsAsync(new ApiResponse<User>(new HttpResponseMessage(HttpStatusCode.OK), user, new RefitSettings()));
    }
}