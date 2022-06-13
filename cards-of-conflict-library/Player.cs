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
    public bool Connected => messageManager.Client.Connected;
    public Boolean HostPlayer { get; }

    public void AddPoint()
    {
        Points += 1;
    }

    internal void Notify(string text)
    {
        if (messageManager != null)
        {
            var message = new Message(MessageType.SendMessage);
            message.Text = text;
            messageManager.SendMessage(message);
        }
        else
        {
            Console.WriteLine(text);
        }
    }

    internal void AddWinPoint()
    {
        Points += 1;
    }

    internal void SendCards(IEnumerable<WhiteCard> cards)
    {
        if (messageManager != null)
        {
            var message = new Message(MessageType.SendCards);
            message.Attachment = cards;
            messageManager.SendMessage(message);
        }
        else
        {
            Cards.AddRange(cards);
        }
    }
}
