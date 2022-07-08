using CardsOfConflict.Windows.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CardsOfConflict.Windows.GUI
{
    /// <summary>
    /// Interaction logic for HostPage.xaml
    /// </summary>
    public partial class HostPage : Page
    {
        private GameViewModel model { get => DataContext as GameViewModel; }

        public HostPage(GameViewModel model)
        {
            DataContext = model;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (model.LobbyStarted)
            {
                model.AbortGame();
            }
            else
            {
                if (model.DeckList.Any(x => x.Enabled))
                    model.HostNewGame();
                else
                    model.Info = "No Deck Selected";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (model.LobbyStarted)
            {
                model.AbortGame();
            }

            model.GoBack();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (sender is null)
                return;
            var button = sender as Button;
            if (button is null)
                return;

            string ip = 
                button.Tag.ToString() == "local" ? 
                    model.Network.LocalIp.ToString() : 
                    model.Network.ExternalIp.ToString();

            Clipboard.SetText(ip);
            model.Info = $"Copied {ip} in clipboard";
        }
    }
}
