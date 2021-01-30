using System;
using System.Net.Http;
using System.Threading.Tasks;
using Boardgames.BlazorClient.Brookers;
using Boardgames.BlazorClient.Services;
using Boardgames.Client.Brookers;
using Boardgames.Client.Caches;
using Boardgames.Client.Factories;
using Boardgames.Client.Services;
using Boardgames.Client.ViewModels;
using Boardgames.Common.Services;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Boardgames.BlazorClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("Boardgames.WebServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Boardgames.WebServerAPI"));

            builder.Services.AddApiAuthorization();

            RegisterServices(builder, builder.Services);
            RegisterViewModels(builder.Services);
            await builder.Build().RunAsync();
        }

        private static void RegisterServices(WebAssemblyHostBuilder builder, IServiceCollection services)
        {
            services.AddSingleton<IIconUriProvider>(new IconUriProvider(new Uri(builder.HostEnvironment.BaseAddress)));
            services.AddSingleton<DragDropDataStore>();

            services.AddSingleton<IMessenger, Messenger>();

            services.AddScoped<IWebApiBrooker, WebApiBrooker>();
            services.AddScoped<ISignalRBrooker>(x => new SignalRBrooker(
                new Uri(builder.HostEnvironment.BaseAddress),
                x.GetRequiredService<IMessenger>(),
                x.GetRequiredService<IAccessTokenProvider>()));

            services.AddScoped<IPlayerDataProvider, PlayerDataProvider>();
            services.AddSingleton<IPlayerDataCache, PlayerDataCache>();

            services.AddScoped<IGameInfoService, GameInfoService>();
            services.AddScoped<INinthPlanetService, NinthPlanetService>();

            services.AddScoped<BrowserService>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<MainViewModel>();

            services.AddTransient<GameBrowserViewModel>();
            services.AddTransient<CreateGameViewModel>();
            services.AddTransient<HomeViewModel>();

            services.AddScoped<INinthPlanetScreenViewModelFactory, NinthPlanetScreenViewModelFactory>();
        }
    }
}