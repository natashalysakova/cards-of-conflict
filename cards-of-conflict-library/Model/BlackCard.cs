using CardsOfConflict.Library.Enums;
namespace CardsOfConflict.Library.Model;

[Serializable]
<<<<<<< HEAD
internal class BlackCard : Card
=======
class BlackCard : Card
>>>>>>> master
{
    public BlackCard(string text) : base(text)
    {
        var answers = text.Count(x => x == '_');
        AnswersNumber = answers == 0 ? 1 : answers;
    }

    public int AnswersNumber { get; set; }
    public override CardType Type => CardType.Black;
}
