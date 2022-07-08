using CardsOfConflict.Library.Model;
using System.Collections.ObjectModel;

namespace CardsOfConflict.Library.Game;

public interface IPlayer
{
    void Notify(string text);
    void SendCards(IEnumerable<WhiteCard> cards);
    IEnumerable<WhiteCard> GetAnswers(int answersNumber);
    int GetWinner(int answersNumber);
    void NewRound(int round);
    void GameOver();
    void AddPoint();
    void Stop();


    string Name { get; }

    int Points { get; }

    bool IsTsar { get; set; }
    ObservableCollection<WhiteCard> Cards { get; }
    string Info { get; set; }
    event EventHandler InfoChanged;

    void GameStarted(string[] players);
}