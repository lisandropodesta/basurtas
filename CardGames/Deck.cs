using System;
using System.Collections.Generic;

namespace CardGames
{
    /// <summary>
    /// Card deck.
    /// </summary>
    public class Deck<TS, TR>
    {
        private readonly List<Card<TS, TR>> cards = new List<Card<TS, TR>>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public Deck()
        {
            foreach (var suit in Enum.GetValues(typeof(TS)))
            {
                foreach (var rank in Enum.GetValues(typeof(TR)))
                {
                    var card = new Card<TS, TR>
                    {
                        Suit = (TS)suit,
                        Rank = (TR)rank
                    };

                    cards.Add(card);
                }
            }
        }

        public void Mix()
        {
        }
    }
}
