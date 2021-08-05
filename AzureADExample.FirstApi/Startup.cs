using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace AzureADExample.FirstApi
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

            services.AddControllers();
            services.AddHttpClient();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AzureADExample.FirstApi", Version = "v1" });
            });

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                {
                    opts.Authority = "https://login.microsoftonline.com/d33894ee-4acb-44de-8514-40aacd156434/v2.0/";
                    opts.Audience = "api://FirstApiApplicationIdUri";
                    opts.TokenValidationParameters.ValidIssuers = new List<string>()
                    {
                        "https://login.microsoftonline.com/d33894ee-4acb-44de-8514-40aacd156434/v2.0",
                        "https://sts.windows.net/d33894ee-4acb-44de-8514-40aacd156434/"
                    };
                })
                // Fill with b2c data
                .AddJwtBearer("AzureB2CExample", opts =>
                {
                    opts.Authority = "https://login.microsoftonline.com/d33894ee-4acb-44de-8514-40aacd156434/v2.0/";
                    opts.Audience = "api://FirstApiApplicationIdUri";
                    opts.TokenValidationParameters.ValidIssuers = new List<string>()
                    {
                        "https://login.microsoftonline.com/d33894ee-4acb-44de-8514-40aacd156434/v2.0",
                        "https://sts.windows.net/d33894ee-4acb-44de-8514-40aacd156434/"
                    };
                });

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                        JwtBearerDefaults.AuthenticationScheme,
                        "AzureB2CExample")
                    .RequireAuthenticatedUser();

                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AzureADExample.FirstApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", (context) => context.Response.WriteAsync("FirstApi"));

            });
        }
    }
}
