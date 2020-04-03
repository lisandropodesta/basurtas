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

            UpdateAllRoundScores();

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

            Rounds[currRound].Player[playerIndex].Bid = quantity;
        }

        /// <summary>
        /// Add one baza to player.
        /// </summary>
        public void AddBazaToPlayer(byte playerIndex)
        {
            if (playerIndex >= playersCount)
            {
                throw new ArgumentException("Invalid player");
            }

            UpdateScore(playerIndex, 1);
        }

        /// <summary>
        /// Updates score of the round for all players.
        /// </summary>
        private void UpdateAllRoundScores()
        {
            for (var i = 0; i < playersCount; i++)
            {
                UpdateScore(i, 0);
            }
        }

        /// <summary>
        /// Update a player score optionally adding bazas.
        /// </summary>
        private void UpdateScore(int playerIndex, byte addBazas)
        {
            var playerScore = Rounds[currRound].Player[playerIndex];
            playerScore.Bazas = (byte)((playerScore.Bazas ?? 0) + addBazas);

            var prev = (byte)(currRound > 0 ? Rounds[currRound - 1].Player[playerIndex].Score ?? 0 : 0);
            playerScore.Score = (byte)(prev + playerScore.Bazas + (playerScore.Bid == playerScore.Bazas ? 10 : 0));
        }
    }
}
