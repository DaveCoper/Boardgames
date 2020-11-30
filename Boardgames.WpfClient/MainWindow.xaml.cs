using System.Windows;

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
        }

        private void OnAvatarImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }
    }
}