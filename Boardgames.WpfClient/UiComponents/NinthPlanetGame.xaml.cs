using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Boardgames.WpfClient.UiComponents
{
    /// <summary>
    /// Interaction logic for NinthPlanetGame.xaml
    /// </summary>
    public partial class NinthPlanetGame : UserControl
    {
        public readonly static DependencyProperty CardPlayedCommandProperty = DependencyProperty.Register(
            nameof(CardPlayedCommand),
            typeof(ICommand),
            typeof(NinthPlanetGame));

        public readonly static DependencyProperty DisplayCardCommandProperty = DependencyProperty.Register(
            nameof(DisplayCommand),
            typeof(ICommand),
            typeof(NinthPlanetGame));

        public NinthPlanetGame()
        {
            InitializeComponent();
        }

        public ICommand CardPlayedCommand
        {
            get => (ICommand)GetValue(CardPlayedCommandProperty);
            set => SetValue(CardPlayedCommandProperty, value);
        }

        public ICommand DisplayCommand
        {
            get => (ICommand)GetValue(DisplayCardCommandProperty);
            set => SetValue(DisplayCardCommandProperty, value);
        }
    }
}