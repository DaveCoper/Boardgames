using System;
using System.Net.Http;
using System.Threading.Tasks;
using Boardgames.Shared.Brookers;
using Boardgames.Shared.Services;
using Boardgames.Web.Shared;
using Boardgames.Wpf.Client.Brookers;
using Boardgames.Wpf.Client.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boardgames.Wpf.Client.ViewModels
{
    public class AppContext
    {
        public AppContext()
        {
            SetupDiContainer();
        }

        public ServiceProvider ServiceProvider { get; private set; }

        public IConfigurationRoot Configuration { get; private set; }

        public async Task BeforeStart()
        {
            var booker = ServiceProvider.GetRequiredService<IWebApiBrooker>();
            var weather = await booker.GetAsync<WeatherForecast[]>("WeatherForecast");
        }

        private static void RegisterServices(ServiceCollection services)
        {
            services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();
            services.AddSingleton<ISecureStore, SecureStore>();
            services.AddSingleton<IUserDataStore, UserDataStore>();
            services.AddSingleton<IWebApiBrooker, WebApiBrooker>();
            services.AddSingleton<IFileStore, WpfFileStore>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<HttpClient, HttpClient>();
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

            services.AddTransient<LoginViewModel>();
            services.AddSingleton<Func<LoginViewModel>>(x => () => x.GetRequiredService<LoginViewModel>());
        }
    }
}