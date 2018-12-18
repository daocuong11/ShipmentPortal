using ApiTest1.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTest1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            var authConfig = Configuration.GetSection("Authentication").Get<AuthenticationConfig>();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = authConfig.Authority;
                    options.RequireHttpsMetadata = false;

                    options.ApiName = authConfig.ApiName;
                    options.ApiSecret = authConfig.ApiSecret;
                });

            // CORS
            //services.AddCors(c => c.AddPolicy("testCors",
            //    builder => builder
            //        .AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
 
            // CORS
            //app.UseCors("testCors");

            // Authentication
            app.UseAuthentication();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
