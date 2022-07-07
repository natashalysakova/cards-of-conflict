using CardsOfConflict.Windows.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
