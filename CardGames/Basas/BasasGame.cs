namespace CardGames.Basas
{
    /// <summary>
    /// Game of Basas.
    /// </summary>
    public class BasasGame : CardGame
    {
        /// <summary>
        /// Min players number.
        /// </summary>
        public override int MinPlayersNumber => 2;

        /// <summary>
        /// Max players number.
        /// </summary>
        public override int MaxPlayersNumber => 7;

        // Deck of cards.
        private readonly Deck<EnglishCardSuit, EnglishCardRank> deck = new Deck<EnglishCardSuit, EnglishCardRank>();

        // Scoreboard.
        public BasasScoreboard Scoreboard { get; private set; }

        /// <summary>
        /// Game started.
        /// </summary>
        protected override void GameStarted()
        {
            Scoreboard = new BasasScoreboard(PlayersNumber);
        }
    }
}
