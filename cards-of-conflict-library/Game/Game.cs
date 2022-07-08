using CardsOfConflict.Library.Extentions;
using CardsOfConflict.Library.Helpers;
using CardsOfConflict.Library.Model;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;

namespace CardsOfConflict.Library.Game
{

    public class Game : IDisposable
    {
        public delegate void GameStartedDelegate(string[] players);

        ObservableCollection<IPlayer> players;
        CancellationTokenSource cancellationTokenSource;
        public event GameStartedDelegate GameStarted;

        public Game(ObservableCollection<IPlayer> players)
        {
            this.players = players;
        }
        public Game() : this(new ObservableCollection<IPlayer>())
        {
        }

        const int port = 2022;

        int maxPlayers;
        TcpListener server;
        NormalGame game;

        private void NotifyPlayers(string message)
        {
            foreach (var player in players)
            {
                player.Notify(message);
            }
        }

        public void HostNewGame()
        {
            var externalIp = NetworkHelper.GetPublicIpAddress();
            var localIp = NetworkHelper.GetLocalIPAddress();

            string myPlayerName = string.Empty;
#if DEBUG
            maxPlayers = 2;
            myPlayerName = "Host";
#else

        while (true)
        {
            try
            {
                Console.WriteLine("How many players do you want?");
                maxPlayers = int.Parse(s: Console.ReadLine());
                break;
            }
            catch
            {
                Console.WriteLine("Invalid number");
            }
        }

        Console.WriteLine("Enter your name");
        myPlayerName = Console.ReadLine();
#endif
            Console.WriteLine("Select Deck (use coma to separate few decks):");

            var availableDecks = Deck.GetDeckList();

            for (int i = 0; i < availableDecks.Count(); i++)
            {
                Console.WriteLine($"{i + 1} {availableDecks.ElementAt(i)}");
            }

            var selected = new List<int>();
            if (availableDecks.Count() == 1)
            {
                selected.Add(1);
            }
            else
            {
                var selectedString = Console.ReadLine() ?? string.Empty;
                selected = selectedString.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
            }

            Console.WriteLine("Selected decks: ");
            var deck = new Deck();

            if (!selected.Any())
            {
                selected.Add(1);
            }

            foreach (var i in selected)
            {
                var deckName = availableDecks.ElementAt(i - 1);
                Console.WriteLine($"{deckName}");
                deck.AddCards(deckName);
            }
            HostNewGame(localIp, externalIp, maxPlayers, new HostPlayer(myPlayerName), deck, new CancellationTokenSource());
        }

        public void Abort()
        {
            cancellationTokenSource.Cancel();
            players.Clear();
        }

        public async void HostNewGame(IPAddress localIp, IPAddress externalIp, int maxPlayers, IPlayer player, Deck deck, CancellationTokenSource cancellationToken)
        {
            this.cancellationTokenSource = cancellationToken;

            player.Info = $"Your local ip: {localIp}:{port}";
            player.Info = $"Your public ip: {externalIp}:{port}";
            player.Info = "Public Ip Copied to clipboard";
            player.Info = "Starting Host";

            players.Add(player);
            server = new TcpListener(localIp, port);
            server.Start();

            while (players.Count < maxPlayers)
            {
                NotifyPlayers("Waiting for players");

                var client = server.AcceptTcpClientAsync(cancellationToken.Token);
              
                try
                {
                    var messageMamager = new MessageManager(client.Result);
                    messageMamager.RequestName();
                    var data = messageMamager.GetNextMessage();

                    var playerName = data.Text;
                    players.Add(new RemotePlayer(playerName, messageMamager));
                    NotifyPlayers($"{playerName} joined the game");
                }
                catch (Exception)
                {
                    if (client.IsCanceled)
                    {
                        player.Info = $"game was cancelled";
                        server.Stop();
                        return;
                    }
                }
            }

            player.Info = $"{players.Count} joined.";
            var names = players.Select(x => x.Name).ToArray();
            StartGame(names);
            

            ServerGameLoop(deck);


            server.Stop();
        }

        private void StartGame(string[] local)
        {
            GameStarted?.Invoke(local);
            foreach (var player in players)
            {
                player.GameStarted(local);
            }
        }

        public int JoinTheGame()
        {

            string ipAddress = string.Empty;
#if DEBUG
            ipAddress = NetworkHelper.GetLocalIPAddress().ToString();
#else
        while (true)
        {
            try
            {
                Console.WriteLine("Enter ip address:");
                ipAddress = Console.ReadLine();
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid ip adress");
                continue;
            }
        }
#endif
            game = new NormalGame();
            game.RequestAnswers += Game_RequestAnswers;
            game.MessageRecived += Game_MessageRecived;
            game.SelectWinner += Game_SelectWinner;
            game.NextRound += Game_NextRound;
            game.GameOver += Game_GameOver;
            return game.JoinTheGame(ipAddress, port);
        }

        private void Game_GameOver()
        {
            Console.WriteLine("Game over");
        }

        private void Game_NextRound(int round, IEnumerable<WhiteCard> cards)
        {
            Console.Clear();
            Console.WriteLine($"====== Round {round} ======");
            Console.WriteLine("My Cards");
            for (int i = 0; i < cards.Count(); i++)
            {
                Console.WriteLine($"{i + 1}. {cards.ElementAt(i)}");
            }
        }

        private int Game_SelectWinner(int numberOfPlayers)
        {
            int winner;
            while (true)
            {
                Console.WriteLine("Select a winner:");
                if (int.TryParse(Console.ReadLine(), out winner))
                {
                    if (winner > numberOfPlayers || winner < 1)
                    {
                        Console.WriteLine("Wrong answer");
                    }
                    else
                    {
                        break;
                    }
                }

            }
            return winner;
        }

        private void Game_MessageRecived(string message)
        {
            Console.WriteLine(message);
        }

        private IEnumerable<int> Game_RequestAnswers(int numberOfAnswers)
        {
            Console.WriteLine($"Choose {numberOfAnswers} answers");
            for (int i = 0; i < numberOfAnswers; i++)
            {
                int id;
                while (true)
                {
                    Console.WriteLine($"Select answer #{i + 1}:");
                    if (int.TryParse(Console.ReadLine(), out id))
                    {
                        break;
                    }
                }
                yield return id - 1;
            }
        }

        private void ServerGameLoop(Deck deck)
        {
            NotifyPlayers("===================================");
            NotifyPlayers("Everyone joined. Starting the Game!");
            NotifyPlayers("===================================");

            var firstTsar = new Random().Next(maxPlayers);

            var round = 0;
            foreach (var player in players)
            {
                var cards = deck.TakeWhiteCards(10);
                player.SendCards(cards);
            }

            for (int i = firstTsar; i <= maxPlayers; i++)
            {
                if (i == maxPlayers)
                {
                    i = 0;
                }

                Console.WriteLine($"Round {round++}");
                NotifyNewRound(round);



                var tsar = SetATsar(players[i]);
                NotifyPlayers("===================================");
                NotifyPlayers($"{tsar.Name} is Tsar");

                var card = deck.TakeBlackCard();
                NotifyPlayers($"[ROUND CARD] {card.Text}");

                var answers = new Dictionary<IPlayer, IEnumerable<WhiteCard>>();
                var tasks = new List<Task>();

                NotifyPlayers("===================================");
                NotifyPlayers("Waiting for players answers");
                NotifyPlayers("===================================");
                foreach (var player in players)
                {
                    if (player != tsar)
                    {
                        tasks.Add(
                            Task.Run(() =>
                            {
                                var answer = player.GetAnswers(card.AnswersNumber);
                                answers.Add(player, answer);
                                NotifyPlayers($"{player.Name} sent an answer");
                            }));
                    }
                }

                Task.WaitAll(tasks.ToArray());

                var shuffledPlayers = players.Where(x => x != tsar).Shuffle();

                for (int j = 0; j < shuffledPlayers.Count(); j++)
                {
                    var answer = answers[shuffledPlayers.ElementAt(j)];

                    NotifyPlayers("===================================");
                    NotifyPlayers($"Answer #{j + 1}");
                    NotifyPlayers($"[ROUND CARD] {card.Text}");
                    foreach (var answerCard in answer)
                    {
                        NotifyPlayers($"{answerCard}");
                        deck.ReturnWhiteCard(answerCard);
                    }
                }

                NotifyPlayers($"Waiting for {tsar.Name} to decide a winner");
                var winnerNumber = tsar.GetWinner(answers.Count);

                var winnerPlayer = shuffledPlayers.ElementAt(winnerNumber - 1);
                winnerPlayer.AddPoint();
                NotifyPlayers($"And the point goes to {winnerPlayer.Name}");

                deck.ReturnBlackCard(card);
                foreach (var player in players)
                {
                    var count = 10 - player.Cards.Count;
                    if (count > 0)
                    {
                        var cards = deck.TakeWhiteCards(count);
                        player.SendCards(cards);
                        NotifyPlayers($"{player.Name} took {count} cards");
                    }

                }

                Console.WriteLine("Press ENTER for next round");
                Console.WriteLine("Press ESC for finish");
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    var winner = players.Where(x => x.Points == players.Max(x => x.Points));
                    NotifyPlayers("===================================");
                    NotifyPlayers($"GAME IS OVER");
                    foreach (var player in winner)
                    {
                        NotifyPlayers($"{player} has {player.Points} points!");
                    }
                    NotifyPlayers("===================================");
                    NotifyGameOver();

                    break;
                }
            }

        }

        private void NotifyGameOver()
        {
            foreach (var player in players)
            {
                player.GameOver();
            }
        }

        private void NotifyNewRound(int round)
        {
            foreach (var player in players)
            {
                player.NewRound(round);
            }
        }

        private IPlayer SetATsar(IPlayer player)
        {
            foreach (var p in players)
            {
                p.IsTsar = p.Name == player.Name;
            }
            return player;
        }

        public void Dispose()
        {
            foreach (var player in players)
            {
                player.Stop();
            }

            if (game != null)
            {
                game.Dispose();
            }

            if (server != null)
            {
                server.Stop();
            }
        }
    }
}