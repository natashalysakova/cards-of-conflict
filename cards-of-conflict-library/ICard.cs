interface ICard
{
    CardType Type { get; }
    string Text { get; set; }
    public Guid ID { get; }
}
