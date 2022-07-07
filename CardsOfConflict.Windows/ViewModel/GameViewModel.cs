using CardsOfConflict.Library.Game;
using CardsOfConflict.Library.Helpers;
using CardsOfConflict.Windows.GUI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CardsOfConflict.Windows.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public GameViewModel()
        {
            Network = new NetworkModel();
            Players = new ObservableCollection<Player>();
            DeckList = new ObservableCollection<KeyValuePair<string, bool>>(Deck.GetDeckList().Select(x => new KeyValuePair<string, bool>(x, false)));
        }

        public string Origin { get; set; }

        private Page activePage;

        public Page ActivePage
        {
            get
            {
                if (activePage is null)
                {
                    activePage = App.ServiceProvider.GetService<StartupPage>();
                    OnPropertyChanged(nameof(ActivePage));
                }
                return activePage;
            }
            set
            {
                activePage = value;
                OnPropertyChanged(nameof(ActivePage));
            }
        }

        internal void HostNewGame()
        {
            this.Game = new Game(Players);
            var deck = new Deck();
            foreach (var d in DeckList)
            {
                if (d.Value)
                {
                    deck.AddCards(d.Key);
                }
            }

            //Task.Run(() => { 
                Game.HostNewGame(Network.LocalIp, Network.ExternalIp, MaxPlayers, PlayerName, deck); 
            // });
        }

        public NetworkModel Network { get; set; }
        public Game Game { get; set; }

        private string playerName;

        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; OnPropertyChanged(nameof(PlayerName)); }
        }
        private int maxPlayers;

        public int MaxPlayers
        {
            get { return maxPlayers; }
            set { maxPlayers = value; OnPropertyChanged(nameof(MaxPlayers)); }
        }

        public ObservableCollection<Player> Players { get; }
        public ObservableCollection<KeyValuePair<string, bool>> DeckList { get; }

        #region INotifyPropertyChanged implemenation
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public class NetworkModel
        {
            int port;
            public IPAddress ExternalIp
            {
                get => NetworkHelper.GetPublicIpAddress();
            }
            public IPAddress LocalIp
            {
                get => NetworkHelper.GetLocalIPAddress();
            }
            public int Port
            {
                get
                {
                    if (port == 0)
                    {
                        port = int.Parse(ConfigurationManager.AppSettings["defaultPort"]);
                        OnPropertyChanged(nameof(Port));
                    }
                    return port;
                }
                set
                {
                    port = value;
                    ConfigurationManager.AppSettings["defaultPort"] = port.ToString();
                    OnPropertyChanged(nameof(Port));
                }
            }

            

            

            #region INotifyPropertyChanged implemenation
            public event PropertyChangedEventHandler? PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}
