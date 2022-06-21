using CardsOfConflict.Library.Enums;
using CardsOfConflict.Library.Interfaces;
namespace CardsOfConflict.Library.Model;

[Serializable]
<<<<<<< HEAD
public abstract class Card : ICard
=======
abstract class Card : ICard
>>>>>>> master
{
    public Card(string text)
    {
        Text = text;
    }
<<<<<<< HEAD
    public abstract CardType Type { get; }
=======
    abstract public CardType Type { get; }
>>>>>>> master
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
