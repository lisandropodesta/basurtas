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
        public Player CurrentPlayer => playerAtPosition?[currPlayerIndex];

        /// <summary>
        /// Card representing the trinfo.
        /// </summary>
        public Card<EnglishCardSuit, EnglishCardRank> Triunfo { get; private set; }

        /// <summary>
        /// Played cards on this hand on the current round.
        /// </summary>
        public Card<EnglishCardSuit, EnglishCardRank>[] PlayedCards { get; private set; }

        /// <summary>
        /// Last player flag.
        /// </summary>
        protected bool IsLastPlayer => currFirstPlayer == GetNextPlayerIndex();

        private List<Player> playerAtPosition;

        private byte currentRound;

        private byte leftHandsToPlay;

        private byte currFirstPlayer;

        private byte currPlayerIndex;

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
        /// Game started.
        /// </summary>
        protected override void StartGame()
        {
            Scoreboard = new BasasScoreboard(PlayersNumber);

            playerAtPosition = Players.ToArray().ToList();
            // TODO: mix players

            PlayedCards = new Card<EnglishCardSuit, EnglishCardRank>[PlayersNumber];

            currentRound = 0;
            currFirstPlayer = 0;
            currPlayerIndex = 0;

            NextStep(false);
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
                var currRoundCards = BasasConsts.RoundCards[currentRound];

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
        /// Plays a card.
        /// </summary>
        public void PlayCard(Player player, Card<EnglishCardSuit, EnglishCardRank> card)
        {
            if (State == BasasState.Play)
            {
                var index = GetPlayerIndex(player);

                if (currPlayerIndex == index && playerCards[index].Contains(card))
                {
                    PlayedCards[index] = card;
                    playerCards[index].Remove(card);
                    NextStep(true);
                }
            }
        }

        /// <summary>
        /// Continues paused game.
        /// </summary>
        public void Continue()
        {
            if (State == BasasState.HandFinished)
            {
                NextStep(false);
            }
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
        /// Go to next step of the game.
        /// </summary>
        private void NextStep(bool skip)
        {
            if (skip)
            {
                currPlayerIndex = GetNextPlayerIndex();
            }

            if (currPlayerIndex == currFirstPlayer)
            {
                switch (State)
                {
                    case BasasState.Build:
                        StartRound();
                        break;

                    case BasasState.Bid:
                        State = BasasState.Play;
                        break;

                    case BasasState.Play:
                        FinishHand();
                        break;

                    case BasasState.HandFinished:
                        ContinueAfterFinishingHand();
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
        private void StartRound()
        {
            currFirstPlayer = currPlayerIndex = GetPlayerIndex(currentRound);
            leftHandsToPlay = BasasConsts.RoundCards[currentRound];
            DealCards();
            State = BasasState.Bid;
            Scoreboard.RoundBegins(currentRound);
        }

        /// <summary>
        /// Finish the hand and assign points.
        /// </summary>
        private void FinishHand()
        {
            State = BasasState.HandFinished;

            leftHandsToPlay--;

            var playerIndex = CalcHandWinner();
            Scoreboard.AddBasaToPlayer(playerIndex);
            currFirstPlayer = currPlayerIndex = playerIndex;
        }

        /// <summary>
        /// Continue after finishing a hand.
        /// </summary>
        private void ContinueAfterFinishingHand()
        {
            ResetPlayerCards();

            if (leftHandsToPlay > 0)
            {
                State = BasasState.Play;
            }
            else if (currentRound < BasasConsts.RoundsNumber)
            {
                currentRound++;
                StartRound();
            }
            else
            {
                State = BasasState.GameFinished;
            }
        }

        private void ResetPlayerCards()
        {
            for (var i = 0; i < PlayersNumber; i++)
            {
                PlayedCards[i] = null;
            }
        }

        /// <summary>
        /// Calculates winner.
        /// </summary>
        private byte CalcHandWinner()
        {
            byte winningPlayerIndex = 0;
            Card<EnglishCardSuit, EnglishCardRank> winningCard = null;

            for (var pi = 0; pi < PlayersNumber; pi++)
            {
                var playerIndex = GetPlayerIndex(pi + currFirstPlayer);
                var playerCard = PlayedCards[playerIndex];

                if (pi == 0 || IsNewCardWinner(winningCard, playerCard))
                {
                    winningCard = playerCard;
                    winningPlayerIndex = playerIndex;
                }
            }

            return winningPlayerIndex;
        }

        /// <summary>
        /// Check if a new card is the new winner.
        /// </summary>
        private bool IsNewCardWinner(Card<EnglishCardSuit, EnglishCardRank> winningCard, Card<EnglishCardSuit, EnglishCardRank> newCard)
        {
            if (newCard.Suit == Triunfo.Suit && winningCard.Suit != Triunfo.Suit)
            {
                return true;
            }

            if (newCard.Suit == winningCard.Suit && newCard.Rank > winningCard.Rank)
            {
                return true;
            }

            return false;
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
                var playerIndex = GetPlayerIndex(pi + currFirstPlayer);
                var cardsPerPlayer = BasasConsts.RoundCards[currentRound];

                for (var ri = 0; ri < cardsPerPlayer; ri++)
                {
                    var card = mixedDeck.GetNextCard();

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
