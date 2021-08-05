using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AzureADExample.B2CApiClient
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AzureADExample.B2CApiClient", Version = "v1" });
            });

            services.AddDistributedMemoryCache();

            // services.Configure<CookiePolicyOptions>(options =>
            // {
            //     // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //     options.CheckConsentNeeded = context => true;
            //     options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            //     // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
            //     options.HandleSameSiteCookieCompatibility();
            // });
            //
            // services.AddOptions();
            //
            // services
            //     .AddMicrosoftIdentityWebApiAuthentication(Configuration, "AzureAdB2C")
            //     .EnableTokenAcquisitionToCallDownstreamApi()
            //     .AddInMemoryTokenCaches();
            //
            // services.Configure<OpenIdConnectOptions>(options =>
            // {
            //     options.Authority = "https://azureadb2cmwalczynski.b2clogin.com/azureadb2cmwalczynski.onmicrosoft.com/B2C_1_AzureADExample/v2.0/";
            //     options.ClientId = "c766a2e4-640c-42fe-9d31-eb099ae8bf83";
            //     options.ClientSecret = "_33O.y~NUvn1WB.PcEHl76.ywB3d-ySzgc";
            //     options.SaveTokens = true;
            // });


            // This works, but doesn't look like a best solution
            //
            services
                .AddAuthentication(opts =>
                {
                    opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opts =>
                {
                     opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                     opts.Authority = "https://azureadb2cmwalczynski.b2clogin.com/azureadb2cmwalczynski.onmicrosoft.com/B2C_1_AzureADExample/v2.0/";
                     opts.ClientId = "c766a2e4-640c-42fe-9d31-eb099ae8bf83";
                     opts.ClientSecret = "_33O.y~NUvn1WB.PcEHl76.ywB3d-ySzgc";
                     opts.ResponseType = OidcConstants.ResponseTypes.IdToken;
                     opts.SaveTokens = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AzureADExample.B2CApiClient v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", (context) => context.Response.WriteAsync("B2CApiClient"));
            });
        }
    }
}
