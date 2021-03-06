using CardsOfConflict.Library.Enums;
using CardsOfConflict.Library.Interfaces;
namespace CardsOfConflict.Library.Model;

[Serializable]
public abstract class Card : ICard
{
    public Card(string text)
    {
        Text = text;
    }
    public abstract CardType Type { get; }
    public string Text { get; set; }
    public Guid ID { get; private set; }

    public void SetNewId()
    {
        ID = Guid.NewGuid();
    }

    public override string ToString()
    {
        return Text;
    }
}
