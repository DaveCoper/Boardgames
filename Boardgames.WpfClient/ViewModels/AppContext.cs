using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Threading;
using Boardgames.Client.Brookers;
using Boardgames.Client.Caches;
using Boardgames.Client.Services;
using Boardgames.Client.ViewModels;
using Boardgames.Client.ViewModels.NinthPlanet;
using Boardgames.Common.Models;
using Boardgames.Common.Services;
using Boardgames.WpfClient.Brookers;
using Boardgames.WpfClient.Services;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boardgames.WpfClient.ViewModels
{
    public class AppContext
    {
        public AppContext(Dispatcher dispatcher)
        {
            SetupDiContainer(dispatcher);
        }

        public ServiceProvider ServiceProvider { get; private set; }

        public IConfigurationRoot Configuration { get; private set; }

        public async Task BeforeStart()
        {
            var playerDataService = ServiceProvider.GetRequiredService<IPlayerDataProvider>();
            await playerDataService.GetPlayerDataForCurrentUserAsync();

            var signalR = ServiceProvider.GetRequiredService<ISignalRBrooker>();
            await signalR.ConnectAsync(default);
        }

        private void SetupDiContainer(Dispatcher dispatcher)
        {
            var builder = new ConfigurationBuilder();
            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, dispatcher);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services, Dispatcher dispatcher)
        {
            services.AddSingleton(dispatcher);

            RegisterWindows(services);
            RegisterServices(services);
            RegisterViewModels(services);
        }

        private void RegisterViewModels(ServiceCollection services)
        {
            services.AddTransient<MainViewModel>();

            services.AddTransient<LoginViewModel>();
            services.AddSingleton<Func<LoginViewModel>>(x => () => x.GetRequiredService<LoginViewModel>());

            services.AddTransient<GameBrowserViewModel>();
            services.AddTransient<CreateGameViewModel>();
            services.AddTransient<HomeViewModel>();

            services.AddSingleton<Func<GameInfo, PlayerData, GameInfoViewModel>>(
                x => (gameInfo, gameOwner) =>
                {
                    return new GameInfoViewModel(
                        gameInfo,
                        gameOwner,
                        x.GetRequiredService<IIconUriProvider>(),
                        x.GetRequiredService<IMessenger>());
                });
        }

        private void RegisterServices(ServiceCollection services)
        {
            services.AddSingleton<IIconUriProvider, IconUriProvider>();

            services.AddSingleton<IFileStore, WpfFileStore>();
            services.AddSingleton<IDialogService, DialogService>();

            services.AddSingleton<IMessenger, Messenger>();

            services.AddSingleton<ISecureStore, SecureStore>();
            services.AddSingleton<IUserDataStore, UserDataStore>();

            services.AddSingleton<HttpClient, HttpClient>();
            services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();
            services.AddSingleton<IWebApiBrooker, WebApiBrooker>();
            services.AddSingleton<ISignalRBrooker, SignalRBrooker>();

            services.AddSingleton<IPlayerDataProvider, PlayerDataProvider>();
            services.AddSingleton<IPlayerDataCache, PlayerDataCache>();

            services.AddSingleton<IGameInfoService, GameInfoService>();
            services.AddSingleton<INinthPlanetService, NinthPlanetService>();
        }

        private void RegisterWindows(ServiceCollection services)
        {
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(LoginWindow));
        }
    }
}