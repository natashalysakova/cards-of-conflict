using System.Net.Sockets;
using CardsOfConflict.Library.Extentions;
using CardsOfConflict.Library.Helpers;
using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game
{

    public class Game : IDisposable
    {
        private const int port = 2022;
        private readonly List<IPlayer> players = new();
        private int maxPlayers;
        private TcpListener? server;
        private NormalGame? game;


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
            Console.WriteLine($"Your local ip: {localIp}:{port}");
            Console.WriteLine($"Your public ip: {externalIp}:{port}");
            Console.WriteLine("Public Ip Copied to clipboard");
            Console.WriteLine("Starting Host");

            string myPlayerName = String.Empty;
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
            players.Add(new HostPlayer(myPlayerName));
            server = new TcpListener(localIp, port);
            server.Start();

            while (players.Count < maxPlayers)
            {
                NotifyPlayers("Waiting for players");

                var client = server.AcceptTcpClient();
                var messageMamager = new MessageManager(client);
                messageMamager.RequestName();
                var data = messageMamager.GetNextMessage();

                var playerName = data.Text ?? $"player{players.Count + 1}";
                players.Add(new RemotePlayer(playerName, messageMamager));
                NotifyPlayers($"{playerName} joined the game");
            }

            Console.WriteLine($"{players.Count} joined.");


            Console.WriteLine("Select Deck (use coma to separate few decks):");

            var availableDecks = GetDeckList();

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
                var selectedString = Console.ReadLine() ?? String.Empty;
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




            ServerGameLoop(deck);


            server.Stop();
        }

        private static IEnumerable<string> GetDeckList()
        {
            var path = Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Decks"));
            foreach (var item in path)
            {
                var info = new DirectoryInfo(item);
                yield return info.Name;
            }
        }

        public int JoinTheGame()
        {

            string ipAddress = string.Empty;
#if DEBUG
            ipAddress = "192.168.0.101";
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
            return game.JoinTheGame(ipAddress, port);
        }

        void ServerGameLoop(Deck deck)
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
                    i = 0;
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

        IPlayer SetATsar(IPlayer player)
        {
            foreach (var p in players)
            {
                if (p.Name == player.Name)
                    p.IsTsar = true;
                else
                    p.IsTsar = false;
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
                game.Dispose();

            if (server != null)
                server.Stop();
        }
    }
}