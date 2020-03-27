using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGames.Basas
{
    /// <summary>
    /// Game of Basas.
    /// </summary>
    public class BasasGame : CardGame
    {
        /// <summary>
        /// Min players number.
        /// </summary>
        public override int MinPlayersNumber => 2;

        /// <summary>
        /// Max players number.
        /// </summary>
        public override int MaxPlayersNumber => 7;

        // Deck of cards.
        private readonly Deck<EnglishCardSuit, EnglishCardRank> deck = new Deck<EnglishCardSuit, EnglishCardRank>();

        /// <summary>
        /// General state.
        /// </summary>
        public BasasState State { get; private set; }

        /// <summary>
        /// Scoreboard.
        /// </summary>
        public BasasScoreboard Scoreboard { get; private set; }

        /// <summary>
        /// Current player.
        /// </summary>
        public Player CurrentPlayer { get; private set; }

        /// <summary>
        /// Card representing the trinfo.
        /// </summary>
        public Card<EnglishCardSuit, EnglishCardRank> Triunfo { get; private set; }

        /// <summary>
        /// Last player flag.
        /// </summary>
        protected bool IsLastPlayer => currFirstPlayer == GetNextPlayerIndex();

        private List<Player> playerAtPosition;

        private byte currentRound;

        private byte currPlayerIndex;

        private byte currFirstPlayer;

        // Current player cards.
        private List<Card<EnglishCardSuit, EnglishCardRank>>[] playerCards;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BasasGame()
        {
            State = BasasState.Build;
        }

        /// <summary>
        /// Get current player cards.
        /// </summary>
        public List<Card<EnglishCardSuit, EnglishCardRank>> GetCards(Player player)
        {
            var index = GetPlayerIndex(player);
            return index >= 0 ? playerCards[index] : null;
        }

        /// <summary>
        /// Gets a card file name.
        /// </summary>
        public string GetCardFile(Card<EnglishCardSuit, EnglishCardRank> card)
        {
            return $"/Images/EnglishCards/{card.Suit.ToString().ToLower()}_{GetText(card.Rank)}.svg";
        }

        /// <summary>
        /// Set a player bid for the current round.
        /// </summary>
        public void SetPlayerBid(Player player, int bid)
        {
            if (player == CurrentPlayer)
            {
                var currRoundCards = BasasConsts.RoundCards[currentRound - 1];

                var validBid = bid <= currRoundCards;

                if (validBid && IsLastPlayer)
                {
                    var count = Scoreboard.GetRoundBid() + bid;
                    validBid = count != currRoundCards;
                }

                if (!validBid)
                {
                    throw new Exception(string.Format("No podés pedir {0}.", bid));
                }

                Scoreboard.SetPlayerBid(currPlayerIndex, (byte)bid);
                NextStep(true);
            }
        }

        /// <summary>
        /// Game started.
        /// </summary>
        protected override void StartGame()
        {
            Scoreboard = new BasasScoreboard(PlayersNumber);

            playerAtPosition = Players.ToArray().ToList();

            // TODO: mix players

            currentRound = 0;
            currFirstPlayer = 0;
            currPlayerIndex = 0;

            NextStep(false);
            NotifyStateChanged();
        }

        /// <summary>
        /// Check if the player is allowed to enter.
        /// </summary>
        protected override bool IsAllowedToEnter(Player player, out string reason)
        {
            if (State != BasasState.Build)
            {
                reason = "El juego ya comenzó";
                return false;
            }

            return base.IsAllowedToEnter(player, out reason);
        }

        /// <summary>
        /// Go to next game step.
        /// </summary>
        private void NextStep(bool skip)
        {
            if (skip)
            {
                currPlayerIndex = GetNextPlayerIndex();
            }

            CurrentPlayer = playerAtPosition[currPlayerIndex];

            if (currPlayerIndex == currFirstPlayer)
            {
                switch (State)
                {
                    case BasasState.Build:
                        StartNextRound();
                        break;

                    case BasasState.Bid:
                        State = BasasState.Play;
                        break;

                    case BasasState.Play:
                        if (currentRound == BasasConsts.RoundsNumber)
                        {
                            State = BasasState.End;
                        }
                        else
                        {
                            StartNextRound();
                        }
                        break;

                    default:
                        throw new Exception("Fatal game error!");
                }
            }

            NotifyStateChanged();
        }

        /// <summary>
        /// Start biding in the next round.
        /// </summary>
        private void StartNextRound()
        {
            currentRound++;
            DealCards();
            State = BasasState.Bid;
            Scoreboard.RoundBegins(currentRound);
        }

        /// <summary>
        /// Deal all round cards.
        /// </summary>
        private void DealCards()
        {
            var mixedDeck = deck.StartDealingCards();

            playerCards = new List<Card<EnglishCardSuit, EnglishCardRank>>[PlayersNumber];

            for (var pi = 0; pi < PlayersNumber; pi++)
            {
                var cardsPerPlayer = BasasConsts.RoundCards[currentRound - 1];
                for (var ri = 0; ri < cardsPerPlayer; ri++)
                {
                    var card = mixedDeck.GetNextCard();
                    var playerIndex = GetPlayerIndex(pi + currFirstPlayer);

                    if (ri == 0)
                    {
                        playerCards[playerIndex] = new List<Card<EnglishCardSuit, EnglishCardRank>>();
                    }

                    playerCards[playerIndex].Add(card);
                }
            }

            Triunfo = mixedDeck.GetNextCard();
        }

        /// <summary>
        /// Get a player.
        /// </summary>
        private Player GetPlayer(int index)
        {
            return playerAtPosition[GetPlayerIndex(index)];
        }

        /// <summary>
        /// Get the next player of the round.
        /// </summary>
        private byte GetNextPlayerIndex()
        {
            return GetPlayerIndex(currPlayerIndex + 1);
        }

        /// <summary>
        /// Get a player index.
        /// </summary>
        private byte GetPlayerIndex(int index)
        {
            return (byte)(index % PlayersNumber);
        }

        /// <summary>
        /// Get a player index.
        /// </summary>
        private byte GetPlayerIndex(Player player)
        {
            return (byte)playerAtPosition.IndexOf(player);
        }

        private static string GetText(EnglishCardRank rank)
        {
            switch (rank)
            {
                case EnglishCardRank.Two:
                    return "2";

                case EnglishCardRank.Three:
                    return "3";

                case EnglishCardRank.Four:
                    return "4";

                case EnglishCardRank.Five:
                    return "5";

                case EnglishCardRank.Six:
                    return "6";

                case EnglishCardRank.Seven:
                    return "7";

                case EnglishCardRank.Eight:
                    return "8";

                case EnglishCardRank.Nine:
                    return "9";

                case EnglishCardRank.Ten:
                    return "10";

                case EnglishCardRank.Jack:
                    return "J";

                case EnglishCardRank.Queen:
                    return "Q";

                case EnglishCardRank.King:
                    return "K";

                case EnglishCardRank.Ace:
                    return "A";

                default:
                    return null;
            }
        }
    }
}
