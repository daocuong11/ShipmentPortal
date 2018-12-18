using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extensions
{
    public static class AzureAdB2CAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder)
            => builder.AddAzureAdB2C(new AzureAdB2CConfig());

        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder, AzureAdB2CConfig b2CConfig)
        {
            builder.AddOpenIdConnect("B2C", options =>
            {
                options.ClientId = b2CConfig.ClientId;
                options.Authority = b2CConfig.Authority;
                options.UseTokenLifetime = true;
                options.CallbackPath = b2CConfig.CallbackPath;
                options.SignedOutCallbackPath = b2CConfig.SignedOutCallbackPath;
            });
            return builder;
        }
    }
}
