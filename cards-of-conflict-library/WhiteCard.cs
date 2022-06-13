[Serializable]
class WhiteCard : Card
{
    public WhiteCard(string text) : base(text) { }
    internal WhiteCard() : base() { }

    public override CardType Type => CardType.White;
}
