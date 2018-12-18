using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extensions
{
    public static class AzureAdAuthenticationBuilderExtensions
    {        
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
            => builder.AddAzureAd(new AzureAdConfig());

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, AzureAdConfig azureConfig)
        {
            builder.AddOpenIdConnect("AD", options =>
            {
                options.ClientId = azureConfig.ClientId;
                options.Authority = azureConfig.Authority;   // V2 specific
                options.UseTokenLifetime = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateIssuer = false;     // accept several tenants
                options.CallbackPath = azureConfig.CallbackPath;
                options.SignedOutCallbackPath = azureConfig.SignedOutCallbackPath;
            });
            return builder;
        }
    }
}
