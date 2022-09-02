using Downcast.Common.Data.Validators.PasswordValidator.Config;
using Downcast.PasswordManager.Model;
using Downcast.UserManager.Client.DependencyInjection;

namespace Downcast.PasswordManager.API.Config;

public static class ServicesConfigurationExtensions
{
    public static WebApplicationBuilder AddPasswordManagerServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPasswordManager, PasswordManager>();

        builder.Services.AddPasswordValidatorOptions(builder.Configuration);

        builder.Services.AddUserManagerClient(builder.Configuration);
        return builder;
    }
}