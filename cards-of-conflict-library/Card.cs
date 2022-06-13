[Serializable]
abstract class Card : ICard
{
    public Card(string text)
    {
        Text = text;
    }

    internal Card()
    {
        Text = ID.ToString();
    }

    abstract public CardType Type { get; }
    public string Text { get; set; }
    public Guid ID => Guid.NewGuid();
    public override string ToString()
    {
        return Text;
    }
}
