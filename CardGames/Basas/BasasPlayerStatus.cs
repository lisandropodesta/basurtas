using System.Collections.Generic;

namespace CardGames.Basas
{
    public class BasasPlayerStatus
    {
        public Player Player { get; private set; }

        public byte Column { get; private set; }

        public List<EnglishCard> Cards => cards;

        private readonly List<EnglishCard> cards = new List<EnglishCard>();

        public BasasPlayerStatus(byte column, Player player)
        {
            Column = column;
            Player = player;
        }
    }
}
