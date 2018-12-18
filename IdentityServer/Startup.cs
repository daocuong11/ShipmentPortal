using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Extensions;
using IdentityServer.Models;
using IdentityServer.Services;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfiguration>(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddAzureAd(Configuration.GetSection("IdentityProvider:AzureAd").Get<AzureAdConfig>())
                .AddAzureAdB2C(Configuration.GetSection("IdentityProvider:AzureAdB2C").Get<AzureAdB2CConfig>())
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.Headers["Location"] = context.RedirectUri;
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                });

            // Identity Server
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(IdentityConfig.GetIdentityResources())
                .AddInMemoryApiResources(IdentityConfig.GetApiResources(Configuration))
                .AddInMemoryClients(IdentityConfig.GetClients(Configuration))
                .AddProfileService<IdentityWithAdditionalClaimsProfileService>();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddSessionStateTempDataProvider(); ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            TurnOffMicrosoftJWTMapping();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseAuthentication();

            // Identity Server
            app.UseIdentityServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void TurnOffMicrosoftJWTMapping()
        {
            //The long claim names come from Microsoft’s JWT handler trying to map some claim types to .NET’s ClaimTypes class types. 
            //We can turn off this behavior with the following line of code (in Startup).
            //The purpose is to change the claim System.Security.Claims.ClaimTypes.NameIdentifier to IdentityModel.JwtClaimTypes.Subject
            // to match with IdentityServerAuthenticationService
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        #region Identity Config

        private static class IdentityConfig
        {
            public static IEnumerable<ApiResource> GetApiResources(IConfigurationRoot config)
            {
                var appConfig = config.GetSection("Application").Get<AppConfig>();

                var apis = appConfig?.APIs.Select(a =>
                    new ApiResource(a.Name, a.DisplayName)
                    {
                        ApiSecrets = a.Secrets.Select(s => new Secret(s.Sha256())).ToList()
                    });

                return apis;
            }

            public static IEnumerable<Client> GetClients(IConfigurationRoot config)
            {
                var appConfig = config.GetSection("Application").Get<AppConfig>();

                var clients = appConfig?.Clients.Select(c =>
                    new Client()
                    {
                        ClientId = c.ClientId,
                        ClientName = c.ClientName,
                        RequireConsent = false,
                        AllowRememberConsent = true,
                        AccessTokenType = AccessTokenType.Reference,
                        AccessTokenLifetime = 36000, // 10 hours
                        IdentityTokenLifetime = 30,
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AlwaysIncludeUserClaimsInIdToken = true,
                        AllowAccessTokensViaBrowser = true,
                        UpdateAccessTokenClaimsOnRefresh = true,
                        AllowOfflineAccess = true,
                        RedirectUris = c.RedirectUris,
                        PostLogoutRedirectUris = c.PostLogoutRedirectUris,
                        AllowedCorsOrigins = c.AllowedCorsOrigins,
                        AllowedScopes = c.AllowedScopes
                    });

                return clients;
            }

            public static IEnumerable<IdentityResource> GetIdentityResources()
            {
                return new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Email()
                };
            }

            public static List<TestUser> GetUsers(IHostingEnvironment env)
            {
                if (!env.IsDevelopment())
                {
                    return new List<TestUser>();
                }

                return new List<TestUser> { new TestUser { SubjectId = "1", Username = "apitest", Password = "1234" }, };
            }
        }

        #endregion
    }
}
