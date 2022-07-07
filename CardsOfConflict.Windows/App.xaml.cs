using CardsOfConflict.Windows.GUI;
using CardsOfConflict.Windows.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CardsOfConflict.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ServiceProvider serviceProvider;

        public static ServiceProvider ServiceProvider { get => serviceProvider; set => serviceProvider = value; }

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            //services.AddSingleton(new GameViewModel() { Origin = "service Provdier"});
            services.AddSingleton<MainWindow>();
            services.AddSingleton<GameViewModel>(new GameViewModel() { Origin = "servieProvider" });
            services.AddScoped<ConnectPage>();
            services.AddScoped<HostPage>();
            services.AddScoped<GamePage>();
            services.AddScoped<StartupPage>();
            services.AddScoped<Settings>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow window = ServiceProvider.GetService<MainWindow>();
            window.DataContext = serviceProvider.GetService<GameViewModel>();
            window.Show();
        }
    }
}
