using CardsOfConflict.Windows.GUI.Controls;
using CardsOfConflict.Windows.ViewModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace CardsOfConflict.Windows.GUI
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private GameViewModel model { get => DataContext as GameViewModel; }

        public GamePage(GameViewModel model)
        {
            DataContext = model;
            InitializeComponent();
        }
    }
}
