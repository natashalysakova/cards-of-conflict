using CardsOfConflict.Library.Model;
using System.Collections.ObjectModel;

namespace CardsOfConflict.Library.Game;

public abstract class Player : IPlayer
{
    private string info;

    //public Player(string name, TcpClient client) : this (name, new MessageManager(client)) { }

    public Player(string name)
    {
        Name = name;
        Cards = new ObservableCollection<WhiteCard>();
    }

    public string Name { get; }
    public int Points { get; private set; }
    public bool IsTsar { get; set; }
    public ObservableCollection<WhiteCard> Cards { get; }
    public string Info { get => info; set { info = value; InfoChanged?.Invoke(this, new EventArgs()); } }
    public event EventHandler InfoChanged;

    public void AddPoint()
    {
        Points += 1;
    }

    public abstract void GameOver();
    public abstract IEnumerable<WhiteCard> GetAnswers(int answersNumber);
    public abstract int GetWinner(int answersNumber);
    public abstract void NewRound(int round);
    public abstract void Notify(string text);
    public abstract void SendCards(IEnumerable<WhiteCard> cards);
    public abstract void Stop();
}
