using CardsOfConflict.Library.Enums;
namespace CardsOfConflict.Library.Model;

[Serializable]
public class Message
{
    public Message(MessageType type)
    {
        Type = type;
    }
    public MessageType Type { get; set; }
    public string? Text { get; set; }
    public dynamic? Attachment { get; set; }
    public int CardNumber { get; set; }

}
