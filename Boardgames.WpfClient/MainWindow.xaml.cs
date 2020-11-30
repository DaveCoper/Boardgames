using System.Windows;
using Boardgames.WpfClient.ViewModels;

namespace Boardgames.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            try
            {
                if (DataContext is IAsyncLoad asyncLoad)
                {
                    await asyncLoad.LoadDataAsync();
                }
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private void OnAvatarImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}