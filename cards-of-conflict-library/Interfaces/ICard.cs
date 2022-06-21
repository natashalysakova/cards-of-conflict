using CardsOfConflict.Library.Enums;
namespace CardsOfConflict.Library.Interfaces;

<<<<<<< HEAD
internal interface ICard
=======
interface ICard
>>>>>>> master
{
    CardType Type { get; }
    string Text { get; set; }
    Guid ID { get; }
    void SetNewId();
}
