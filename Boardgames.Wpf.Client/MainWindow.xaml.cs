using System.Windows;
using Boardgames.Wpf.Client.ViewModels;

namespace Boardgames.Wpf.Client
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
    }
}