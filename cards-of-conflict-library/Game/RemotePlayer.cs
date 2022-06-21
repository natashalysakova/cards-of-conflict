using CardsOfConflict.Library.Enums;
using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game;

class RemotePlayer : Player
{
    public RemotePlayer(string name, MessageManager manager) : base(name)
    {
        messageManager = manager;
    }

    readonly MessageManager messageManager;

    override public void Notify(string text)
    {
        messageManager.SendTextMessage(text);
    }

    override public void SendCards(IEnumerable<WhiteCard> cards)
    {
        messageManager.SendCards(cards);
        Cards.AddRange(cards);
    }

    override public IEnumerable<WhiteCard> GetAnswers(int answersNumber)
    {
        var taken = new List<WhiteCard>();

        messageManager.RequestCards(answersNumber);

        for (int i = 0; i < answersNumber; i++)
        {
            var response = messageManager.GetNextMessage();
            taken.Add(response.Attachment);
        }

        foreach (var card in taken)
        {
            Cards.RemoveAll(x => x.ID == card.ID);
        }
        return taken;
    }

    override public int GetWinner(int answersNumber)
    {

        
        messageManager.SendWinner(answersNumber);

        var response = messageManager.GetNextMessage();
        return response.Attachment;

    }

    override public void NewRound(int round)
    {
        messageManager.NewRound(round);
    }

    override public void GameOver()
    {
        messageManager.GameOver();
    }

    override public void Stop()
    {
        messageManager.Dispose();
    }
}
