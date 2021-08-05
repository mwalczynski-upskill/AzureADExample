using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureADExample.WebClient.CodeFlow
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
            services.AddControllersWithViews();
            services.AddHttpClient();

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
                    opts.Authority = "https://login.microsoftonline.com/d33894ee-4acb-44de-8514-40aacd156434/v2.0/";
                    opts.ClientId = "32497ca2-dcf0-4241-9fef-6d5139db0c2c";
                    opts.ClientSecret = "332ysLw7i46o_2A6ON5V9.UW3ZB4R..lzE";
                    opts.ResponseType = OidcConstants.ResponseTypes.Code; ;
                    opts.SaveTokens = true;

                    // Choose one or the other
                    opts.Scope.Add("api://FirstApiApplicationIdUri/FullAccessFirstApi");
                    // opts.Scope.Add("api://SecondApiApplicationIdUri/FullAccessSecondApi");
                });

            // CANNOT SPECIFY MULTIPLE RESOURCES WITH CODE FLOW !!! chyba, jakos trzeba to zrobic
            // https://stackoverflow.com/questions/46825753/azure-oauth-login-was-working-now-getting-aadsts700022-aadsts700023-errors
            // https://stackoverflow.com/questions/45695382/how-do-i-setup-multiple-auth-schemes-in-asp-net-core-2-0
            // https://stackoverflow.com/questions/49694383/use-multiple-jwt-bearer-authentication
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
