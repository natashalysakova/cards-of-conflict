using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using CardsOfConflict.Library.Extentions;
using CardsOfConflict.Library.Helpers;
using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game
{

    public class Game : IDisposable
    {
        public Game()
        {
            players = new List<Player>();
        }

        const int port = 2022;
        List<Player> players;
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
            players.Add(new Player(myPlayerName));
            server = new TcpListener(localIp, port);
            server.Start();

            while (players.Count < maxPlayers)
            {
                NotifyPlayers("Waiting for players");

                var client = server.AcceptTcpClient();
                var messageMamager = new MessageManager(client);
                messageMamager.RequestName();
                var data = messageMamager.GetNextMessage();

                var playerName = data.Text;
                players.Add(new Player(playerName, messageMamager));
                NotifyPlayers($"{playerName} joined the game");
            }

            Console.WriteLine($"{players.Count} joined.");


            Console.WriteLine("Select Deck (use coma to separate few decks):");

            var availableDecks = GetDeckList();

            for (int i = 0; i < availableDecks.Count(); i++)
            {
                Console.WriteLine($"{i + 1} {availableDecks.ElementAt(i)}");
            }

            IEnumerable<int> selected = new List<int>();
            if (availableDecks.Count() == 1)
            {
                selected.Append(1);
            }
            else
            {
                selected = Console.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x));
            }

            Console.WriteLine("Selected decks: ");
            var deck = new Deck();

            if (!selected.Any())
            {
                selected.Append(1);
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

        private IEnumerable<string> GetDeckList()
        {
            var path = Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Decks"));
            foreach (var item in path)
            {
                DirectoryInfo info = new DirectoryInfo(item);
                yield return info.Name;
            }
        }

        //public void ReadFromFile()
        //{
        //    List<WhiteCard> whiteCards = new List<WhiteCard>();
        //    List<BlackCard> blackCards = new List<BlackCard>();

        //    using (var sr = new StreamReader(File.OpenRead("Decks/Main Game/whiteList.txt")))
        //    {
        //        var str = sr.ReadToEnd();

        //        whiteCards.AddRange(str.Split(Environment.NewLine).Select(x => new WhiteCard(x)));
        //    }

        //    using (var sr = new StreamReader(File.OpenRead("Decks/Main Game/blackList.txt")))
        //    {
        //        var str = sr.ReadToEnd();

        //        blackCards.AddRange(str.Split(Environment.NewLine).Select(x => new BlackCard(x)));
        //    }

        //    var str3 = JsonSerializer.Serialize(blackCards);
        //    using (var s = new StreamWriter(File.OpenWrite("black.json")))
        //    {
        //        s.WriteLine(str3);
        //    }

        //    var str2 = JsonSerializer.Serialize(whiteCards);
        //    using (var s = new StreamWriter(File.OpenWrite("white.json")))
        //    {
        //        s.WriteLine(str2);
        //    }
        //}

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



            var isGameActive = true;
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

                var answers = new Dictionary<Player, IEnumerable<WhiteCard>>();
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
                winnerPlayer.AddWinPoint();
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
                    isGameActive = false;
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

        Player SetATsar(Player player)
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