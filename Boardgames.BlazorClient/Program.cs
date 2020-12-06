using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Boardgames.BlazorClient.Brookers;
using Boardgames.BlazorClient.Services;
using Boardgames.Client.Brookers;
using Boardgames.Client.Caches;
using Boardgames.Client.Services;
using Boardgames.Client.ViewModels;
using Boardgames.Common.Models;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

            //services.AddSingleton<IFileStore, WpfFileStore>();
            //services.AddSingleton<IDialogService, DialogService>();

            services.AddSingleton<IMessenger, Messenger>();

            services.AddScoped<IWebApiBrooker, WebApiBrooker>();
            services.AddScoped<ISignalRBrooker>(x=> new SignalRBrooker(
                new Uri(builder.HostEnvironment.BaseAddress),
                x.GetRequiredService<IMessenger>(),
                x.GetRequiredService<IAccessTokenProvider>()));

            services.AddScoped<IPlayerDataService, PlayerDataService>();
            services.AddSingleton<IPlayerDataCache, PlayerDataCache>();

            services.AddScoped<IGameInfoService, GameInfoService>();
            services.AddScoped<INinthPlanetService, NinthPlanetService>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<MainWindowViewModel>();

            services.AddTransient<GameBrowserViewModel>();
            services.AddTransient<CreateGameViewModel>();
            services.AddTransient<HomeViewModel>();

            services.AddScoped<Func<int, int, NinthPlanet.Models.GameState, NinthPlanetScreenViewModel>>(
                x => (ownerId, gameId, state) =>
                {
                    return new NinthPlanetScreenViewModel(
                        ownerId,
                        gameId,
                        state,
                        x.GetRequiredService<IPlayerDataService>(),
                        x.GetRequiredService<INinthPlanetService>(),
                        x.GetRequiredService<IMessenger>());
                });

            services.AddScoped<Func<GameInfo, PlayerData, GameInfoViewModel>>(
                x => (gameInfo, gameOwner) =>
                {
                    return new GameInfoViewModel(
                        gameInfo,
                        gameOwner,
                        x.GetRequiredService<IIconUriProvider>(),
                        x.GetRequiredService<IMessenger>());
                });
        }
    }
}
