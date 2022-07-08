using CardsOfConflict.Library.Enums;
namespace CardsOfConflict.Library.Model;

[Serializable]
public class WhiteCard : Card
{
    public WhiteCard(string text) : base(text) { }

    public override CardType Type => CardType.White;
}
