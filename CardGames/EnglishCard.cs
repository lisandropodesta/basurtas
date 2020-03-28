namespace CardGames
{
    public class EnglishCard : Card<EnglishCardSuit, EnglishCardRank>
    {
        /// <summary>
        /// Get the suit text.
        /// </summary>
        public override string GetSuitText()
        {
            return Suit.ToString().ToLower();
        }

        /// <summary>
        /// Get the rank text.
        /// </summary>
        public override string GetRankText()
        {
            return GetText(Rank);
        }

        /// <summary>
        /// Get the rank text for an engish card.
        /// </summary>
        public static string GetText(EnglishCardRank rank)
        {
            switch (rank)
            {
                case EnglishCardRank.Two:
                    return "2";

                case EnglishCardRank.Three:
                    return "3";

                case EnglishCardRank.Four:
                    return "4";

                case EnglishCardRank.Five:
                    return "5";

                case EnglishCardRank.Six:
                    return "6";

                case EnglishCardRank.Seven:
                    return "7";

                case EnglishCardRank.Eight:
                    return "8";

                case EnglishCardRank.Nine:
                    return "9";

                case EnglishCardRank.Ten:
                    return "10";

                case EnglishCardRank.Jack:
                    return "J";

                case EnglishCardRank.Queen:
                    return "Q";

                case EnglishCardRank.King:
                    return "K";

                case EnglishCardRank.Ace:
                    return "A";

                default:
                    return null;
            }
        }
    }
}
