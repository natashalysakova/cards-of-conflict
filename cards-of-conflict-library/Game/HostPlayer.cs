using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game;

class HostPlayer : Player
{
    public HostPlayer(string name) : base (name)
    {
    }

    override public void Notify(string text)
    {
        Console.WriteLine(text);
    }

    override public void SendCards(IEnumerable<WhiteCard> cards)
    {
        Cards.AddRange(cards);
    }

    override public IEnumerable<WhiteCard> GetAnswers(int answersNumber)
    {
        var taken = new List<WhiteCard>();

        Console.WriteLine($"Choose {answersNumber} answers");
        for (int i = 0; i < answersNumber; i++)
        {
            int id;
            while (true)
            {
                Console.WriteLine($"Select answer #{i + 1}:");
                if (int.TryParse(Console.ReadLine(), out id))
                    break;
            }

            taken.Add(Cards[id - 1]);
        }

        foreach (var card in taken)
        {
            Cards.RemoveAll(x => x.ID == card.ID);
        }
        return taken;
    }

    override public int GetWinner(int answersNumber)
    {

        int winner;
        while (true)
        {
            Console.WriteLine("Select a winner:");
            if (int.TryParse(Console.ReadLine(), out winner))
            {
                if (winner > answersNumber || winner < 1)
                {
                    Console.WriteLine("Wrong answer");
                }
                else
                {
                    break;
                }
            }

        }
        return winner;
    }

    override public void NewRound(int round)
    {
        Console.Clear();
        Console.WriteLine($"====== Round {round} ======");
        Console.WriteLine("My Cards");
        for (int i = 0; i < Cards.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Cards[i]}");
        }
    }

    override public void GameOver()
    {

    }

    public override void Stop()
    {
        
    }
}
