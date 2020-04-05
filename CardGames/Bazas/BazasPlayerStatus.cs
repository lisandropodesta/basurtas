using System.Collections.Generic;

namespace CardGames.Bazas
{
    public class BazasPlayerStatus
    {
        public Player Player { get; private set; }

        public byte Column { get; private set; }

        public List<EnglishCard> Cards => cards;

        private readonly List<EnglishCard> cards = new List<EnglishCard>();

        public BazasPlayerStatus(byte column, Player player)
        {
            Column = column;
            Player = player;
        }

        public void ReplacePlayer(Player newPlayer)
        {
            Player = newPlayer;
        }
    }
}
