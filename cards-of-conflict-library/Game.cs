using System.Net.Sockets;

public class Game
{
    public Game()
    {
        players = new List<Player>();
    }
    const int port = 2022;
    List<Player> players;
    int maxPlayers;
    TcpListener server;


    

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
        string myPlayerName = Console.ReadLine();
        players.Add(new Player(myPlayerName));

        Console.WriteLine("Starting Host");

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


        ServerGameLoop();


        server.Stop();
    }

    public void JoinTheGame()
    {
        string ipAddress = string.Empty;
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

        new NormalGame().JoinTheGame(ipAddress, port);
    }

    void ServerGameLoop()
    {
        NotifyPlayers("===================================");
        NotifyPlayers("Everyone joined. Starting the Game!");
        NotifyPlayers("===================================");

        

        var isGameActive = true;
        var firstTsar = new Random().Next(maxPlayers);
        var deck = new Deck();

        foreach (var player in players)
        {
            var cards = deck.TakeWhiteCards(10);
            player.SendCards(cards);
        }


        for (int i = firstTsar; i <= maxPlayers; i++)
        {
            if (i == maxPlayers)
                i = 0;

            Console.WriteLine("Press for next round");
            Console.ReadLine();
            //var tsar = SetATsar(players[i]);
            //NotifyPlayers($"{tsar.Name} is Tsar");

            //var card = deck.TakeBlackCard();
            //NotifyPlayers($"[TSAR CARD] {card.Text}");

            //Dictionary<Player, IEnumerable<Card>> answers = new Dictionary<Player, IEnumerable<Card>>();
            //var tasks = new List<Task>();

            //NotifyPlayers("===================================");
            //NotifyPlayers("Waiting for players answers");
            //NotifyPlayers("===================================");
            //foreach (var player in players)
            //{
            //    tasks.Add(
            //    Task.Run(() =>
            //    {
            //        var answer = player.GetAnswers(card.AnswersNumber);
            //        answers.Add(player, answer);
            //        Console.WriteLine($"{player.Name} sending answer");
            //    }));
            //}

            //Task.WaitAll(tasks.ToArray());

            //var shuffledPlayers = players.Shuffle();

            //for (int j = 0; j < shuffledPlayers.Count(); j++)
            //{
            //    var answer = answers[shuffledPlayers.ElementAt(j)];

            //    NotifyPlayers("===================================");
            //    NotifyPlayers($"[TSAR CARD] {card.Text}");
            //    foreach (var answerCard in answer)
            //    {
            //        NotifyPlayers($"{j}. {answerCard}");
            //    }
            //}

            //var winnerNumber = tsar.GetWinner();

            //shuffledPlayers.ElementAt(winnerNumber).AddWinPoint();


            //var winner = players.SingleOrDefault(x => x.Points > 10);
            //if (winner != null)
            //{
            //    isGameActive = false;
            //    NotifyPlayers("===================================");
            //    NotifyPlayers($"GAME IS OVER. {winner.Name} is a winner!");
            //    NotifyPlayers("===================================");

            //    break;
            //}


            if (players.Count(x => x.Connected) == 0)
                break;
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
}

public class NormalGame
{
    TcpClient client;
    string myPlayerName;
    MessageManager messageManager;

    public NormalGame()
    {
        client = new TcpClient();
        messageManager = new MessageManager(client);
    }

    public void JoinTheGame(string ipAddress, int port)
    {
        while (true)
        {
            try
            {
                client.Connect(ipAddress, port);
                Console.WriteLine("Connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot connect to {ipAddress}");
                Console.WriteLine(ex.Message);
                continue;
            }

            GameLoop();
        }



    }

    void GameLoop()
    {
        
        var isGameActive = true;
        var cards = new List<WhiteCard>();

        while (isGameActive)
        {
            Thread.Sleep(100);

            var nexMessage = messageManager.GetNextMessage();
            switch (nexMessage.Type)
            {
                case MessageType.SendCards:
                    cards.AddRange(nexMessage.Attachment);
                    Console.WriteLine("My Cards");
                    for (int i = 0; i < cards.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {cards[i]}");
                    }
                    break;
                case MessageType.GetCards:

                    break;
                case MessageType.SendMessage:
                    var message = nexMessage.Text;
                    Console.WriteLine(message);
                    break;
                case MessageType.GetMessage:
                    messageManager.SendTextMessage("??");
                    break;
                case MessageType.GetWinner:

                    break;
                case MessageType.RequestName:
                    Console.WriteLine("Enter your name: ");
                    myPlayerName = Console.ReadLine();
                    messageManager.SendName(myPlayerName);
                    break;
                case MessageType.SendName:
                    break;
                default:
                    break;
            }
        }

    }
}
