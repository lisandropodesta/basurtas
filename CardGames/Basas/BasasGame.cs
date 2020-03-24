using CardGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private BasasScoreboard scoreboard;

        /// <summary>
        /// Game started.
        /// </summary>
        protected override void GameStarted()
        {
            scoreboard = new BasasScoreboard(PlayersNumber);
        }
    }
}
