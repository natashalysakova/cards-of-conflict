using System.Text.Json;
using CardsOfConflict.Library.Extentions;
using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game
{
    [Serializable]
    class Deck
    {
        Stack<BlackCard> blackStack;
        Stack<WhiteCard> whiteStack;

        List<BlackCard> blackCards;
        List<WhiteCard> whiteCards;
        List<BlackCard> usedBlackCards;
        List<WhiteCard> usedWhiteCards;

        internal Stack<BlackCard> BlackCards { get => blackStack; }
        internal Stack<WhiteCard> WhiteCards { get => whiteStack; }

        public Deck()
        {
            blackStack = new Stack<BlackCard>();
            whiteStack = new Stack<WhiteCard>();

            usedBlackCards = new List<BlackCard>();
            usedWhiteCards = new List<WhiteCard>();
            blackCards = new List<BlackCard>();
            whiteCards = new List<WhiteCard>();
        }

        private void Shuffle()
        {
            blackStack = blackCards.ShuffleIntoStack();
            whiteStack = whiteCards.ShuffleIntoStack();
        }

        public BlackCard TakeBlackCard()
        {
            BlackCard result;
            blackStack.TryPop(out result);

            if (result is null)
            {
                blackStack = usedBlackCards.ShuffleIntoStack();
                result = blackStack.Pop();
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
                whiteStack.TryPop(out result);
                if (result is null)
                {
                    whiteStack = usedWhiteCards.ShuffleIntoStack();
                    result = whiteStack.Pop();
                }
                list.Add(result);
            }
            return list;
        }

        public void AddCards(string deckName)
        {
            whiteCards.AddRange(ReadDeck<WhiteCard>($"Decks/{deckName}/white.json"));
            blackCards.AddRange(ReadDeck<BlackCard>($"Decks/{deckName}/black.json"));

            Shuffle();
        }

        private IEnumerable<T> ReadDeck<T>(string path)
        {
            string json;
            using (var s = new StreamReader(File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path))))
            {
                json = s.ReadToEnd();
            }

            return JsonSerializer.Deserialize<IEnumerable<T>>(json);
        }
    }
}