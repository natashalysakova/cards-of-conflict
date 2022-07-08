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
    }
}
