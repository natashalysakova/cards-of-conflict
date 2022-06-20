using CardsOfConflict.Library.Enums;

namespace CardsOfConflict.Library.Model
{
    [Serializable]
    class BlackCard : Card
    {
        public BlackCard(string text) : base(text)
        {
            var answers = text.Count(x => x == '_');
            AnswersNumber = answers == 0 ? 1 : answers;
        }

        //    internal BlackCard() : base("")
        //    {
        //#if DEBUG
        //        AnswersNumber = new Random().Next(1, 3);
        //        Thread.Sleep(100);
        //#endif
        //    }

        public int AnswersNumber { get; set; }
        public override CardType Type => CardType.Black;
    }
}