using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game;

<<<<<<< HEAD
internal abstract class Player : IPlayer
=======
abstract class Player : IPlayer
>>>>>>> master
{
    public Player(string name)
    {
        Name = name;
        Cards = new List<WhiteCard>();
    }

    public string Name { get; }
    public int Points { get; private set; }
    public bool IsTsar { get; set; }
    public List<WhiteCard> Cards { get; }

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
