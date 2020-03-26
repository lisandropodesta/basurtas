using System;

namespace CardGames.Basas
{
    public class BasasScoreboard
    {
        public BasasScoreboardRound[] Rounds { get; private set; }

        private readonly byte playersCount;

        private byte currRound;

        public BasasScoreboard(int playersCount)
        {
            this.playersCount = (byte)playersCount;

            Rounds = new BasasScoreboardRound[BasasConsts.RoundsNumber];

            for (var round = 0; round < BasasConsts.RoundsNumber; round++)
            {
                Rounds[round] = new BasasScoreboardRound(playersCount);
            }
        }

        /// <summary>
        /// Round starts.
        /// </summary>
        public void RoundBegins(byte round)
        {
            if (round != currRound + 1)
            {
                throw new ArgumentException("Invalid round");
            }

            currRound = round;
        }

        /// <summary>
        /// Get the total bid of the current round.
        /// </summary>
        public int GetRoundBid()
        {
            return Rounds[currRound].GetTotalBid();
        }

        /// <summary>
        /// Stored quantity asked for the player.
        /// </summary>
        public void SetPlayerBid(byte playerIndex, byte quantity)
        {
            if (playerIndex >= playersCount)
            {
                throw new ArgumentException("Invalid player");
            }

            Rounds[currRound].Player[playerIndex].Bid = quantity;
        }

        /// <summary>
        /// Stored quantity made for the player.
        /// </summary>
        public void SetPlayerBasas(byte playerIndex, byte quantity)
        {
            if (playerIndex >= playersCount)
            {
                throw new ArgumentException("Invalid player");
            }

            Rounds[currRound].Player[playerIndex].Basas = quantity;
            var prev = currRound >= 2 ? Rounds[currRound - 1].Player[playerIndex].Basas : (byte)0;
            Rounds[currRound].Player[playerIndex].Score = (byte)(prev + quantity + (Rounds[currRound].Player[playerIndex].Bid == quantity ? 10 : 0));
        }
    }
}
