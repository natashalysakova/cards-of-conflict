[Serializable]
abstract class Card : ICard
{
    public Card(string text)
    {
        Text = text;
    }
    abstract public CardType Type { get; }
    public string Text { get; set; }
    public Guid ID { get; set; }
    public override string ToString()
    {
        return Text;
    }
}
