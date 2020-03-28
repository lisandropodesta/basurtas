namespace CardGames
{
    /// <summary>
    /// Card.
    /// </summary>
    public abstract class Card<TS, TR>
    {
        /// <summary>
        /// Suit.
        /// </summary>
        public TS Suit { get; set; }

        /// <summary>
        /// Rank.
        /// </summary>
        public TR Rank { get; set; }

        public override string ToString()
        {
            return string.Format($"{Rank} of {Suit}");
        }

        /// <summary>
        /// Get the suit text.
        /// </summary>
        public abstract string GetSuitText();

        /// <summary>
        /// Get the rank text.
        /// </summary>
        public abstract string GetRankText();
    }
}
