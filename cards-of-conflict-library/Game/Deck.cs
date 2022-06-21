using System.Text.Json;
using CardsOfConflict.Library.Extentions;
using CardsOfConflict.Library.Interfaces;
using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game
{
    [Serializable]
    class Deck
    {
        Stack<BlackCard> blackStack;
        Stack<WhiteCard> whiteStack;

        readonly List<BlackCard> blackCards;
        readonly List<WhiteCard> whiteCards;
        readonly List<BlackCard> usedBlackCards;
        readonly List<WhiteCard> usedWhiteCards;

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
            var popped = blackStack.TryPop(out BlackCard? result);

            if (!popped)
            {
                blackStack = usedBlackCards.ShuffleIntoStack();
                result = blackStack.Pop();
            }

            if (result != null)       
                return result;

            throw new NullReferenceException("Cannot get black card from stack");
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
                var popped = whiteStack.TryPop(out WhiteCard? result);
                if (!popped)
                {
                    whiteStack = usedWhiteCards.ShuffleIntoStack();
                    whiteStack.TryPop(out result);
                }
                if(result != null)
                    list.Add(result);
            }

            return list;
        }

        public void AddCards(string deckName)
        {
            whiteCards.AddRange(ReadDeck<WhiteCard>($"Decks/{deckName}/white.json"));
            blackCards.AddRange(ReadDeck<BlackCard>($"Decks/{deckName}/black.json"));

            fillIds(whiteCards);
            fillIds(blackCards);

            Shuffle();
        }

        private static IEnumerable<T> ReadDeck<T>(string path)
        {
            string json;
            using (var s = new StreamReader(File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path))))
            {
                json = s.ReadToEnd();
            }

            try
            {
                var cards = JsonSerializer.Deserialize<IEnumerable<T>>(json);
                if (cards is null)
                    throw new NullReferenceException("cards is null");
                return cards;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot deserialize deck from {path} : {ex.Message}");
                throw;
            }
        }

        private static void fillIds(IEnumerable<ICard> cards)
        {
            foreach (var card in cards)
            {
                if(card.ID == default)
                {
                    card.SetNewId();
                }
            }
        }
    }
}