using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game;

internal interface IPlayer
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
    List<WhiteCard> Cards { get; }
}