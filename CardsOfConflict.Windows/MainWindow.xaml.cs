using CardsOfConflict.Windows.ViewModel;
using System.Windows;

namespace CardsOfConflict.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GameViewModel model;

        public MainWindow(GameViewModel model)
        {
            InitializeComponent();
            this.model = model;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                model.CardWidth = e.NewSize.Width / 10 - 26;
                e.Handled = true;
            }
            
        }
    }
}
