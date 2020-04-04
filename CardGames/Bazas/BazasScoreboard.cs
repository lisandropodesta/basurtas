using Lit;
using System;

namespace CardGames.Bazas
{
    public class BazasScoreboard
    {
        public BazasScoreboardRound[] Rounds { get; private set; }

        private readonly byte playersCount;

        private bool started;

        private byte currRound;

        public BazasScoreboard(int playersCount)
        {
            this.playersCount = (byte)playersCount;

            Rounds = new BazasScoreboardRound[BazasConsts.RoundsNumber];

            for (var round = 0; round < BazasConsts.RoundsNumber; round++)
            {
                Rounds[round] = new BazasScoreboardRound(playersCount);
            }
        }

        /// <summary>
        /// Round starts.
        /// </summary>
        public void RoundBegins(byte round)
        {
            var target = started ? currRound + 1 : 0;
            if (round != target)
            {
                throw new ArgumentException("Invalid round");
            }

            started = true;
            currRound = round;
        }

        /// <summary>
        /// Round ends.
        /// </summary>
        public void RoundEnds()
        {
            UpdateAllRoundScores();
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

            var ps = Rounds[currRound].Player[playerIndex];
            ps.Bid = quantity;
            ps.Bazas = 0;
            ps.PrevScore = (byte)(currRound > 0 ? Rounds[currRound - 1].Player[playerIndex].Score : 0);
        }

        /// <summary>
        /// Finished current hand.
        /// </summary>
        public void HandEnd(byte winnerPlayerIndex)
        {
            if (winnerPlayerIndex >= playersCount)
            {
                throw new ArgumentException("Invalid player");
            }

            var ps = Rounds[currRound].Player[winnerPlayerIndex];
            ps.Bazas += 1;
        }

        /// <summary>
        /// Updates score of the round for all players.
        /// </summary>
        private void UpdateAllRoundScores()
        {
            for (var playerIndex = 0; playerIndex < playersCount; playerIndex++)
            {
                var ps = Rounds[currRound].Player[playerIndex];
                ps.Score = (byte)(ps.PrevScore + ps.Bazas + (ps.Bid == ps.Bazas ? 10 : 0));
            }
        }
    }
}
