using System;

namespace CardGames.Basas
{
    public class BasasScoreboard
    {
        public BasasScoreboardRound[] Rounds { get; private set; }

        private readonly byte playersCount;

        private bool started;

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
        /// Add one basa to player.
        /// </summary>
        public void AddBasaToPlayer(byte playerIndex)
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
        /// Update a player score optionally adding basas.
        /// </summary>
        private void UpdateScore(int playerIndex, byte addBasas)
        {
            var playerScore = Rounds[currRound].Player[playerIndex];
            playerScore.Basas = (byte)((playerScore.Basas ?? 0) + addBasas);

            var prev = (byte)(currRound > 0 ? Rounds[currRound - 1].Player[playerIndex].Score ?? 0 : 0);
            playerScore.Score = (byte)(prev + playerScore.Basas + (playerScore.Bid == playerScore.Basas ? 10 : 0));
        }
    }
}
