using System.Linq;
using System.Security.Claims;
using Boardgames.Client.Models;
using Boardgames.WebServer.Data;
using Boardgames.WebServer.Hubs;
using Boardgames.WebServer.Models;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Boardgames.WebServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.User.AllowedUserNameCharacters += " ";

                options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
            })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
                {
                    options.Clients.Add(CreateWpfClient());

                    var identityResource = options.IdentityResources["openid"];
                    var apiResource = options.ApiResources.Single();

                    /*
                    identityResource.UserClaims.Add(nameof(ApplicationUser.UserName));
                    apiResource.UserClaims.Add("name");

                    identityResource.UserClaims.Add("role");
                    apiResource.UserClaims.Add("role");
                    */

                    identityResource.UserClaims.Add(nameof(ApplicationUser.Avatar));
                    apiResource.UserClaims.Add(nameof(ApplicationUser.Avatar));

                    identityResource.UserClaims.Add(nameof(ClaimTypes.NameIdentifier));
                    apiResource.UserClaims.Add(nameof(ClaimTypes.NameIdentifier));
                });

             services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AdditionalUserClaimsPrincipalFactory>();

            services.AddAuthentication()
                .AddIdentityServerJwt()
                /*.AddEVEOnline(options=>
                {
                    options.ClientId = Configuration["EVEOnline:ClientId"];
                    options.ClientSecret = Configuration["EVEOnline:ClientSecret"];
                })*/;

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSignalR();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
            });

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>());

            // TODO: Figure out why login page and API have different claims.
            services.Configure<IdentityOptions>(options => options.ClaimsIdentity.UserIdClaimType = "Sub");

            RegisterNinthPlanet(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/hubs/GameHub");
                endpoints.MapFallbackToFile("index.html");
            });
        }

        private static IdentityServer4.Models.Client CreateWpfClient()
        {
            var client = new IdentityServer4.Models.Client
            {
                ClientId = "Boardgames.WpfClient",
                ClientName = "Wpf Client for boardgames",

                RedirectUris = { "http://127.0.0.1:8080/boardgames/" },
                //PostLogoutRedirectUris = { "https://notused" },

                RequireClientSecret = false,

                AllowedGrantTypes = GrantTypes.Code,
                AllowAccessTokensViaBrowser = true,
                RequirePkce = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "Boardgames.WebServerAPI"
                },

                AllowOfflineAccess = true,

                //Access token life time is 7200 seconds (2 hour)
                AccessTokenLifetime = 7200,

                //Identity token life time is 7200 seconds (2 hour)
                IdentityTokenLifetime = 7200,
                RefreshTokenUsage = TokenUsage.ReUse,
            };

            return client;
        }

        private void RegisterNinthPlanet(IServiceCollection services)
        {
            services.AddSingleton<
                Repositories.IGameCache<Games.NinthPlanet>,
                Repositories.DefaultGameCache<Games.NinthPlanet>>();

            services.AddScoped<
                Repositories.INinthPlanetGameRepository,
                Repositories.NinthPlanetGameRepository>();
        }
    }
}