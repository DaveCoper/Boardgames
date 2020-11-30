using System.Windows;
using System.Windows.Controls;
using Boardgames.Client.ViewModels;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.WpfClient.UiComponents
{
    /// <summary>
    /// Interaction logic for NinthPlanet.xaml
    /// </summary>
    public partial class NinthPlanet : UserControl
    {
        public static readonly DependencyProperty GameStateProperty = DependencyProperty.Register(
            nameof(GameState),
            typeof(GameState),
            typeof(NinthPlanet),
            new PropertyMetadata(null, OnGameStateChanged));

        public NinthPlanet()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        public GameState GameState
        {
            get => (GameState)GetValue(GameStateProperty);
            set => SetValue(GameStateProperty, value);
        }

        private static async void OnGameStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thizz = (NinthPlanet)d;
            if (thizz.DataContext is IAsyncLoad asyncLoad)
            {
                await asyncLoad.LoadDataAsync();
            }
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IAsyncLoad asyncLoad)
            {
                await asyncLoad.LoadDataAsync();
            }
        }
    }
}