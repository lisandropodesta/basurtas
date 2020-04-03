using System.Linq;

namespace CardGames.Bazas
{
    public class BazasScoreboardRound
    {
        public BazasScoreboardRoundPlayer[] Player { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BazasScoreboardRound(int playersCount)
        {
            Player = new BazasScoreboardRoundPlayer[playersCount];

            for (var i = 0; i < playersCount; i++)
            {
                Player[i] = new BazasScoreboardRoundPlayer();
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
