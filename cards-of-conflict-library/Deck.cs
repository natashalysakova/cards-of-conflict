[Serializable]
class Deck
{
    Stack<BlackCard> blackCards;
    Stack<WhiteCard> whiteCards;

    List<BlackCard> usedBlackCards;
    List<WhiteCard> usedWhiteCards;

    public Deck()
    {
        blackCards = LoadBlackCards().ShuffleIntoStack();
        whiteCards = LoadWhiteCards().ShuffleIntoStack();

        usedBlackCards = new List<BlackCard>();
        usedWhiteCards = new List<WhiteCard>();
    }

    public BlackCard TakeBlackCard()
    {
        BlackCard result;
        blackCards.TryPop(out result);

        if (result is null)
        {
            blackCards = usedBlackCards.ShuffleIntoStack();
            result = blackCards.Pop();
        }
        return result;
    }

    public void ReturnBlackCard(BlackCard card)
    {
        usedBlackCards.Add(card);
    }
    public void ReturnWhiteCard(WhiteCard card)
    {
        usedWhiteCards.Add(card);
    }
    public void ReturnWhiteCard(IEnumerable<WhiteCard> cards)
    {
        foreach (var card in cards)
        {
            usedWhiteCards.Add(card);
        }
    }

    public IEnumerable<WhiteCard> TakeWhiteCards(int count)
    {
        var list = new List<WhiteCard>();
        for (int i = 0; i < count; i++)
        {
            WhiteCard result;
            whiteCards.TryPop(out result);
            if (result is null)
            {
                whiteCards = usedWhiteCards.ShuffleIntoStack();
                result = whiteCards.Pop();
            }
            list.Add(result);
        }
        return list;
    }

    private IEnumerable<WhiteCard> LoadWhiteCards()
    {
        return new List<WhiteCard>() {
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
             new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
             new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard(),
            new WhiteCard()
        };
    }

    private IEnumerable<BlackCard> LoadBlackCards()
    {
        return new List<BlackCard>() {
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard(),
            new BlackCard()
        };


    }
}
