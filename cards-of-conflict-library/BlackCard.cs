[Serializable]
class BlackCard : Card
{
    public BlackCard(string text) : base(text) { }

    internal BlackCard() : base() { }

    public int AnswersNumber { get; set; }
    public override CardType Type => CardType.Black;
}
