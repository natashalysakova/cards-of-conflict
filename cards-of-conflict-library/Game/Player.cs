using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game;

    public abstract class Player : IPlayer
    {
        //public Player(string name, TcpClient client) : this (name, new MessageManager(client)) { }

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
