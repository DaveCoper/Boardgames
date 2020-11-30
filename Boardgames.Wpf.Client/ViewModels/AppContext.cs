using System;
using System.Net.Http;
using System.Threading.Tasks;
using Boardgames.Shared.Brookers;
using Boardgames.Shared.Caches;
using Boardgames.Shared.Models;
using Boardgames.Shared.Services;
using Boardgames.Shared.ViewModels;
using Boardgames.Web.Shared;
using Boardgames.Wpf.Client.Brookers;
using Boardgames.Wpf.Client.Services;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boardgames.Wpf.Client.ViewModels
{
    public class AppContext
    {
        private PlayerData loggedUser;

        public AppContext()
        {
            SetupDiContainer();
        }

        public ServiceProvider ServiceProvider { get; private set; }

        public IConfigurationRoot Configuration { get; private set; }

        public async Task BeforeStart()
        {
            var booker = ServiceProvider.GetRequiredService<IWebApiBrooker>();
            loggedUser = await booker.GetAsync<PlayerData>("PlayerData");
        }

        private static void RegisterServices(ServiceCollection services)
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

        private static void RegisterWindows(ServiceCollection services)
        {
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(LoginWindow));
        }

        private void SetupDiContainer()
        {
            var builder = new ConfigurationBuilder();
            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            RegisterWindows(services);
            RegisterServices(services);

            services.AddTransient<MainWindowViewModel>();
            
            services.AddTransient<LoginViewModel>();
            services.AddSingleton<Func<LoginViewModel>>(x => () => x.GetRequiredService<LoginViewModel>());

            services.AddTransient<CreateGameViewModel>();
            services.AddSingleton<Func<CreateGameViewModel>>(x => () => x.GetRequiredService<CreateGameViewModel>());            
        }
    }
}