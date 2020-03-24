using System;

namespace CardGames.Basas
{
    public class BasasScoreboard
    {
        private readonly byte playersCount;

        private readonly byte[] asked;

        private readonly byte[] basas;

        private readonly byte[] score;

        private byte currRound;

        public BasasScoreboard(int playersCount)
        {
            this.playersCount = (byte)playersCount;
            asked = new byte[BasasConsts.Rounds * playersCount];
            basas = new byte[BasasConsts.Rounds * playersCount];
            score = new byte[BasasConsts.Rounds * playersCount];
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
        /// Stored quantity asked for the player.
        /// </summary>
        public void PlayerAsks(byte playerIndex, byte quantity)
        {
            if (playerIndex >= playersCount)
            {
                throw new ArgumentException("Invalid player");
            }

            asked[Index(playerIndex)] = quantity;
        }

        /// <summary>
        /// Stored quantity made for the player.
        /// </summary>
        public void PlayerMakes(byte playerIndex, byte quantity)
        {
            if (playerIndex >= playersCount)
            {
                throw new ArgumentException("Invalid player");
            }

            var index = Index(playerIndex);
            basas[index] = quantity;
            var prev = currRound >= 2 ? basas[Index(playerIndex, -1)] : (byte)0;
            score[index] = (byte)(prev + quantity + (asked[index] == quantity ? 10 : 0));
        }

        private int Index(byte playerIndex, int roundShift = 0)
        {
            return (currRound + roundShift - 1) * playersCount + playerIndex;
        }
    }
}
