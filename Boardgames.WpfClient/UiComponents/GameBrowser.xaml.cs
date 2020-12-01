using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Boardgames.Client.ViewModels;

namespace Boardgames.WpfClient.UiComponents
{
    /// <summary>
    /// Interaction logic for GameBrowser.xaml
    /// </summary>
    public partial class GameBrowser : UserControl
    {
        public GameBrowser()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadEntries();
        }

        private async Task LoadEntries()
        {
            this.IsEnabled = false;
            try
            {
                if (this.DataContext is IAsyncLoad asyncLoad)
                {
                    await asyncLoad.LoadDataAsync();
                }
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private async void OnRefreshClicked(object sender, RoutedEventArgs e)
        {
            await LoadEntries();
        }
    }
}