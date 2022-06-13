using System.Net.Sockets;

class Player
{
    //public Player(string name, TcpClient client) : this (name, new MessageManager(client)) { }

    public Player(string name, MessageManager? manager = null)
    {
        if (manager is null)
            HostPlayer = true;
        else
            messageManager = manager;

        Name = name;
        Cards = new List<WhiteCard>();
    }

    public string Name { get; }
    MessageManager messageManager;
    public int Points { get; private set; }
    public bool IsTsar { get; set; }
    public List<WhiteCard> Cards { get; set; }
    //if messageManager is null - player is a host
    public bool Connected => messageManager is null ? true : HealthCheck();

    private bool HealthCheck()
    {
        try
        {
            messageManager.HealthCheck();
            return true;
        }
        catch
        {
            return false;
        }
        
    }

    public Boolean HostPlayer { get; }

    public void AddPoint()
    {
        Points += 1;
    }

    internal void Notify(string text)
    {
        if (HostPlayer)
        {
            Console.WriteLine(text);
        }
        else
        {
            var message = new Message(MessageType.SendMessage);
            message.Text = text;
            messageManager.SendMessage(message);
        }
    }

    internal void AddWinPoint()
    {
        Points += 1;
    }

    internal void SendCards(IEnumerable<WhiteCard> cards)
    {
        if (!HostPlayer)
        {
            var message = new Message(MessageType.SendCards);
            message.Attachment = cards;
            messageManager.SendMessage(message);
        }

        Cards.AddRange(cards);
    }

    internal IEnumerable<WhiteCard> GetAnswers(int answersNumber)
    {
        var taken = new List<WhiteCard>();
        if (HostPlayer)
        {
            // TODO :request cards


            taken.AddRange(Cards.Take(answersNumber));
        }
        else
        {
            var message = new Message(MessageType.GetCards);
            message.CardNumber = answersNumber;
            messageManager.SendMessage(message);

            for (int i = 0; i < answersNumber; i++)
            {
                var response = messageManager.GetNextMessage();
                taken.Add(response.Attachment);
            }

        }


        foreach (var card in taken)
        {
            Cards.RemoveAll(x => x.ID == card.ID);
        }
        return taken;
    }

    internal int GetWinner()
    {
        if (HostPlayer)
        {
            // TODO :rselect

            return 1;

        }
        else
        {
            var message = new Message(MessageType.Winner);
            messageManager.SendMessage(message);

            var response = messageManager.GetNextMessage();
            return response.Attachment;
        }
    }

    internal void NewRound(int round)
    {
        if(HostPlayer)
        {
            Console.Clear();
            Console.WriteLine($"====== Round {round} ======");
            Console.WriteLine("My Cards");
            for (int i = 0; i < Cards.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Cards[i]}");
            }
        }
        else
        {
            var message = new Message(MessageType.NewRound);
            message.Attachment = round;
            messageManager.SendMessage(message);
        }
        
    }

    internal void GameOver()
    {
        if (!HostPlayer)
        {
            var message = new Message(MessageType.GameOver);
            messageManager.SendMessage(message);
        }
    }

    internal void Stop()
    {
        messageManager.Dispose();
    }
}
