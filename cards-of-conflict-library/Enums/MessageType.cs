namespace CardsOfConflict.Library.Enums
{
    [Serializable]
    public enum MessageType
    {
        SendCards = 0,
        GetCards = 1,
        SendMessage = 2,
        GetMessage = 3,
        Winner = 4,
        none = 5,
        SendName = 6,
        RequestName = 7,
        NewRound = 8,
        GameOver = 9
    }
}