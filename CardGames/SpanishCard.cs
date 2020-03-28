namespace CardGames
{
    public class SpanishCard : Card<SpanishCardSuit, SpanishCardRank>
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
        /// Get the rank text for a spanish card.
        /// </summary>
        public static string GetText(SpanishCardRank rank)
        {
            switch (rank)
            {
                case SpanishCardRank.Uno:
                    return "1";

                case SpanishCardRank.Dos:
                    return "2";

                case SpanishCardRank.Tres:
                    return "3";

                case SpanishCardRank.Cuatro:
                    return "4";

                case SpanishCardRank.Cinco:
                    return "5";

                case SpanishCardRank.Seis:
                    return "6";

                case SpanishCardRank.Siete:
                    return "7";

                case SpanishCardRank.Ocho:
                    return "8";

                case SpanishCardRank.Nueve:
                    return "9";

                case SpanishCardRank.Diez:
                    return "10";

                case SpanishCardRank.Once:
                    return "11";

                case SpanishCardRank.Doce:
                    return "12";

                default:
                    return null;
            }
        }
    }
}
