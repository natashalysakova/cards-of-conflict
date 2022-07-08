using CardsOfConflict.Windows.ViewModel;
using System.Windows.Controls;

namespace CardsOfConflict.Windows.GUI
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        private GameViewModel model { get => DataContext as GameViewModel; }

        public Settings(GameViewModel model)
        {
            DataContext = model;
            InitializeComponent();
        }
    }
}
