using CardsOfConflict.Library.Enums;
namespace CardsOfConflict.Library.Model;

[Serializable]
<<<<<<< HEAD
internal class WhiteCard : Card
=======
class WhiteCard : Card
>>>>>>> master
{
    public WhiteCard(string text) : base(text) { }

    public override CardType Type => CardType.White;
}
