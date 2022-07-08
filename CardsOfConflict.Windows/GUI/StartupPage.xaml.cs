using CardsOfConflict.Windows.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace CardsOfConflict.Windows.GUI
{
    /// <summary>
    /// Interaction logic for StartupPage.xaml
    /// </summary>
    public partial class StartupPage : Page
    {
        private GameViewModel model { get => DataContext as GameViewModel; }

        public StartupPage(GameViewModel model)
        {
            DataContext = model;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            model.ActivePage = App.ServiceProvider.GetService<HostPage>();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            model.ActivePage = App.ServiceProvider.GetService<ConnectPage>();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            model.ActivePage = App.ServiceProvider.GetService<Settings>();
        }
    }
}
