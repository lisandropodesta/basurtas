namespace CardGames
{
    /// <summary>
    /// Card.
    /// </summary>
    public class Card<TS, TR>
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
    }
}
