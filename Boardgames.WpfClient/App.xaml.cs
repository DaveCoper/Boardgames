using System.Windows;
using Boardgames.Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Boardgames.WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var context = new ViewModels.AppContext(this.Dispatcher);
            await context.BeforeStart();

            this.MainWindow = context.ServiceProvider.GetRequiredService<MainWindow>();
            this.MainWindow.DataContext = context.ServiceProvider.GetRequiredService<MainViewModel>();
            this.MainWindow.Show();

            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }
}