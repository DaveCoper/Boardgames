﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Threading;
using Boardgames.Client.Brookers;
using Boardgames.Client.Caches;
using Boardgames.Client.Services;
using Boardgames.Client.ViewModels;
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
            var playerDataService = ServiceProvider.GetRequiredService<IPlayerDataService>();
            await playerDataService.GetMyDataAsync();

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
            services.AddTransient<MainWindowViewModel>();

            services.AddTransient<LoginViewModel>();
            services.AddSingleton<Func<LoginViewModel>>(
                x => () => x.GetRequiredService<LoginViewModel>());

            services.AddTransient<CreateGameViewModel>();
            services.AddSingleton<Func<CreateGameViewModel>>(
                x => () => x.GetRequiredService<CreateGameViewModel>());

            services.AddSingleton<Func<int, NinthPlanet.Models.GameState, NinthPlanetScreenViewModel>>(
                x => (ownerId, state) =>
                {
                    return new NinthPlanetScreenViewModel(
                        ownerId,
                        state,
                        x.GetRequiredService<IPlayerDataService>(),
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

            services.AddSingleton<IPlayerDataService, PlayerDataService>();
            services.AddSingleton<IPlayerDataCache, PlayerDataCache>();
        }

        private void RegisterWindows(ServiceCollection services)
        {
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(LoginWindow));
        }
    }
}