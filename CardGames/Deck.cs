using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGames
{
    public interface IMixedDeck<T, TS, TR> where T : Card<TS, TR>
    {
        T GetNextCard();
    }

    /// <summary>
    /// Card deck.
    /// </summary>
    public class Deck<T, TS, TR> where T : Card<TS, TR>, new()
    {
        private readonly List<T> cards = new List<T>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public Deck()
        {
            foreach (var suit in Enum.GetValues(typeof(TS)))
            {
                foreach (var rank in Enum.GetValues(typeof(TR)))
                {
                    var card = new T
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
        private class MixedDeck : IMixedDeck<T, TS, TR>
        {
            private int currentIndex;

            private readonly List<T> cards;

            private readonly int[] position;

            public MixedDeck(List<T> cards)
            {
                this.cards = cards;

                var index = Enumerable.Range(0, cards.Count).ToList();
                Tool.Mix(index);
                position = index.ToArray();
            }

            public T GetNextCard()
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
        public IMixedDeck<T, TS, TR> StartDealingCards()
        {
            return new MixedDeck(cards);
        }
    }
}
