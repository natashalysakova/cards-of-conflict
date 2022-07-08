using CardsOfConflict.Windows.ViewModel;
using System.Windows.Controls;

namespace CardsOfConflict.Windows.GUI
{
    /// <summary>
    /// Interaction logic for ConnectPage.xaml
    /// </summary>
    public partial class ConnectPage : Page
    {
        private GameViewModel model { get => DataContext as GameViewModel; }

        public ConnectPage(GameViewModel model)
        {
            DataContext = model;
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            model.GoBack();
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            model.JoinGame();
        }
    }
}
