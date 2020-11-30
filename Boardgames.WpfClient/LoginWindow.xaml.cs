using System.Threading;
using System.Windows;
using Boardgames.WpfClient.Exceptions;
using Boardgames.WpfClient.ViewModels;

namespace Boardgames.WpfClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private CancellationTokenSource cancellationTokenSource;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel loginViewModel)
            {
                cancellationTokenSource = new CancellationTokenSource();

                try
                {
                    await loginViewModel.LoginAsync(cancellationTokenSource.Token);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (LoginFailedException ex)
                {
                    MessageBox.Show(ex.Message, "Login failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }
    }
}