using CardsOfConflict.Library.Game;
using CardsOfConflict.Windows.GUI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CardsOfConflict.Windows.ViewModel
{
    public partial class GameViewModel : INotifyPropertyChanged
    {
        Stack<Page> navigationStack;
        object _lock = new object();
        private Page activePage;
        private string playerName;
        private int maxPlayers;
        private double cardWidth = 50;
        private string info;
        private bool lobbyStarted;
        private CancellationTokenSource cancellationTokenSource;

        public GameViewModel()
        {
            navigationStack = new Stack<Page>();
            Network = new NetworkModel();
            Players = new ObservableCollection<IPlayer>();
            DeckList = new List<DeckViewModel>(Deck.GetDeckList().Select(x => new DeckViewModel() { Name = x, Enabled = false }));

            BindingOperations.EnableCollectionSynchronization(Players, _lock); 
        }

        public ObservableCollection<IPlayer> Players { get; }
        public IEnumerable<IPlayer> OtherPlayers { get => Players.Except(new List<IPlayer>() { LocalPlayer }); }
        public IEnumerable<DeckViewModel> DeckList { get; }
        public IPlayer LocalPlayer { get; set; }
        public NetworkModel Network { get; set; }
        public Game Game { get; set; }
        public double CardWidth
        {
            get
            {
                return cardWidth;
            }
            set
            {
                cardWidth = value;
                OnPropertyChanged(nameof(CardWidth));
            }
        }
        public string Info
        {
            get => info;
            set { info = value; OnPropertyChanged(nameof(Info)); }
        }
        public string PlayerName
        {
            get
            {
                if (string.IsNullOrEmpty(playerName))
                {
                    PlayerName = ConfigurationManager.AppSettings["playerName"];
                }
                return playerName;
            }
            set
            {
                playerName = value;
                ConfigurationManager.AppSettings["playerName"] = playerName;
                OnPropertyChanged(nameof(PlayerName));
            }
        }
        public Page ActivePage
        {
            get
            {
                if (activePage is null)
                {
                    ActivePage = App.ServiceProvider.GetService<StartupPage>();
                }
                return activePage;
            }
            set
            {
                activePage = value;
                navigationStack.Push(ActivePage);
                OnPropertyChanged(nameof(ActivePage));
            }
        }
        public bool LobbyStarted
        {
            get
            {
                return lobbyStarted;
            }
            set
            {
                lobbyStarted = value;
                OnPropertyChanged(nameof(LobbyStarted));
            }
        }

        public int MaxPlayers
        {
            get
            {
                if (maxPlayers == 0)
                {
                    MaxPlayers = int.Parse(ConfigurationManager.AppSettings["defaultPlayers"]);
                }
                return maxPlayers;
            }
            set
            {
                maxPlayers = value;
                ConfigurationManager.AppSettings["defaultPlayers"] = maxPlayers.ToString();
                OnPropertyChanged(nameof(MaxPlayers));
            }
        }

        internal void Back()
        {
            AbortGame();
            navigationStack.Pop();
            ActivePage = navigationStack.Peek();
        }

        internal void AbortGame()
        {
            Game?.Abort();
            LobbyStarted = false;
            Players.Clear();
        }
        internal void HostNewGame()
        {
            LobbyStarted = true;

            this.Game = new Game(Players);
            var deck = new Deck();
            foreach (var d in DeckList)
            {
                if (d.Enabled)
                {
                    deck.AddCards(d.Name);
                }
            }
            LocalPlayer = new HostPlayer(playerName);
            LocalPlayer.InfoChanged += LocalPlayer_InfoChanged;
            BindingOperations.EnableCollectionSynchronization(LocalPlayer.Cards, _lock);

            cancellationTokenSource = new CancellationTokenSource();
            Game.GameStarted += Game_GameStarted;
            Task.Run(() =>
            {
                Game.HostNewGame(Network.LocalIp, Network.ExternalIp, MaxPlayers, LocalPlayer, deck, cancellationTokenSource);
            });
        }

        private void Game_GameStarted(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                ActivePage = App.ServiceProvider.GetService<GamePage>();
            });
            
        }

        private void LocalPlayer_InfoChanged(object? sender, EventArgs e)
        {
            Info = LocalPlayer.Info;
        }



       

        


        



        #region INotifyPropertyChanged implemenation
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        
    }

    public class CardsToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value / 10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value * 10;
        }
    }
}
