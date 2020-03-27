using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGames
{
    public interface IMixedDeck<TS, TR>
    {
        Card<TS, TR> GetNextCard();
    }

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

        /// <summary>
        /// Local class to support mixed cards dealing.
        /// </summary>
        private class MixedDeck : IMixedDeck<TS, TR>
        {
            private int currentIndex;

            private readonly List<Card<TS, TR>> cards;

            private readonly int[] position;

            public MixedDeck(List<Card<TS, TR>> cards)
            {
                this.cards = cards;
                position = Enumerable.Range(0, cards.Count).ToArray();

                var guid = Guid.NewGuid().ToByteArray();
                var seed = guid[0] + 256 * guid[1] + 65536 * guid[2] + 16777216 * guid[3];
                var rnd = new Random(seed);
                for (var n = 0; n < 100; n++)
                {
                    for (var p1 = 0; p1 < cards.Count; p1++)
                    {
                        var p2 = rnd.Next(0, cards.Count - 1);
                        var prev = position[p1];
                        position[p1] = position[p2];
                        position[p2] = prev;
                    }
                }
            }

            public Card<TS, TR> GetNextCard()
            {
                if (currentIndex >= position.Length)
                {
                    throw new Exception("Mo more cards!");
                }

                return cards[position[currentIndex++]];
            }
        }

        /// <summary>
        /// Creates a mixed cards deck.
        /// </summary>
        public IMixedDeck<TS, TR> StartDealingCards()
        {
            return new MixedDeck(cards);
        }
    }
}
