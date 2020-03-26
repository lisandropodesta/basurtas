using System.Linq;

namespace CardGames.Basas
{
    public class BasasScoreboardRound
    {
        public BasasScoreboardRoundPlayer[] Player { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BasasScoreboardRound(int playersCount)
        {
            Player = new BasasScoreboardRoundPlayer[playersCount];

            for (var i = 0; i < playersCount; i++)
            {
                Player[i] = new BasasScoreboardRoundPlayer();
            }
        }

        /// <summary>
        /// Get the total round bid.
        /// </summary>
        public int GetTotalBid()
        {
            return Player.Sum(p => p.Bid ?? 0);
        }
    }
}
