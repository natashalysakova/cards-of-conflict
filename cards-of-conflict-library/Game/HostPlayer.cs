using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game;

internal class HostPlayer : Player
{
    public HostPlayer(string name) : base (name)
    {
    }

    public override void Notify(string text)
    {
        Console.WriteLine(text);
    }

    public override void SendCards(IEnumerable<WhiteCard> cards)
    {
        Cards.AddRange(cards);
    }

    public override IEnumerable<WhiteCard> GetAnswers(int answersNumber)
    {
        List<WhiteCard> taken = new();

        Console.WriteLine($"Choose {answersNumber} answers");
        for (int i = 0; i < answersNumber; i++)
        {
            int id;
            while (true)
            {
                Console.WriteLine($"Select answer #{i + 1}:");
                if (int.TryParse(Console.ReadLine(), out id))
                {
                    break;
                }
            }

            taken.Add(Cards[id - 1]);
        }

        foreach (WhiteCard card in taken)
        {
            _ = Cards.RemoveAll(x => x.ID == card.ID);
        }
        return taken;
    }

    public override int GetWinner(int answersNumber)
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

    public override void NewRound(int round)
    {
        Console.Clear();
        Console.WriteLine($"====== Round {round} ======");
        Console.WriteLine("My Cards");
        for (int i = 0; i < Cards.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Cards[i]}");
        }
    }

    public override void GameOver()
    {

    }

    public override void Stop()
    {
        
    }
}
