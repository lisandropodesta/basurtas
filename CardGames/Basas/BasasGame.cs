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
        private readonly Deck<EnglishCard, EnglishCardSuit, EnglishCardRank> deck = new Deck<EnglishCard, EnglishCardSuit, EnglishCardRank>();

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
        public Player CurrPlayer => HandPlayers[handPlayerIndex];

        /// <summary>
        /// Number of cards in current round.
        /// </summary>
        public byte CurrRoundCards { get; private set; }

        /// <summary>
        /// Card representing the trinfo.
        /// </summary>
        public EnglishCard Triunfo { get; private set; }

        /// <summary>
        /// Get the round players.
        /// </summary>
        public Player[] HandPlayers { get; private set; }

        /// <summary>
        /// Played cards on this hand on the current round.
        /// </summary>
        public EnglishCard[] HandCards { get; private set; }

        /// <summary>
        /// Last player flag.
        /// </summary>
        protected bool IsLastPlayer => handPlayerIndex == PlayersNumber - 1;

        // Round index, from 0 to 13
        private byte currRound;

        private byte leftHandsToPlay;

        private byte currFirstPlayer;

        // Hand player index, from 0 to number of players - 1
        private byte handPlayerIndex;

        // Players status in sequence of playing rounds.
        private BasasPlayerStatus[] playerStatus;

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
            currRound = 0;

            Scoreboard = new BasasScoreboard(PlayersNumber);

            playerStatus = new BasasPlayerStatus[PlayersNumber];
            HandPlayers = new Player[PlayersNumber];
            HandCards = new EnglishCard[PlayersNumber];

            CreatePlayersList();
            NextStep();
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
        /// Create the list of players.
        /// </summary>
        private void CreatePlayersList()
        {
            var list = Players.ToArray().ToList();

            // TODO: mix players

            for (byte column = 0; column < list.Count; column++)
            {
                playerStatus[column] = new BasasPlayerStatus(column, list[column]);
            }
        }

        /// <summary>
        /// Get current player cards.
        /// </summary>
        public List<EnglishCard> GetCards(Player player)
        {
            if (GetPlayerStatus(player, out BasasPlayerStatus status))
            {
                status.Cards.Sort((c1, c2) =>
                {
                    return 1000 * ((int)c1.Suit - (int)c2.Suit) + ((int)c1.Rank - (int)c2.Rank);
                });

                return status.Cards;
            }

            return null;
        }

        /// <summary>
        /// Gets a card file name.
        /// </summary>
        public string GetCardFile(EnglishCard card)
        {
            return card != null ? $"/Images/EnglishCards/{card.GetSuitText()}_{card.GetRankText()}.svg" :
                $"/Images/EnglishCards/reverse.svg";
        }

        /// <summary>
        /// Set a player bid for the current round.
        /// </summary>
        public void PlaceBid(Player player, int bid)
        {
            if (GetPlayerStatus(player, out BasasPlayerStatus status))
            {
                if (!IsValidBid(player, bid))
                {
                    throw new Exception(string.Format("No podés pedir {0}.", bid));
                }

                Scoreboard.SetPlayerBid(status.Column, (byte)bid);
                NextStep();
            }
        }

        /// <summary>
        /// Check whether a bid is valid or not.
        /// </summary>
        public bool IsValidBid(Player player, int bid)
        {
            var validBid = false;

            if (State == BasasState.Bid && player == CurrPlayer)
            {
                validBid = bid <= CurrRoundCards;

                if (validBid && IsLastPlayer)
                {
                    var count = Scoreboard.GetRoundBid() + bid;
                    validBid = count != CurrRoundCards;
                }
            }

            return validBid;
        }

        /// <summary>
        /// Plays a card.
        /// </summary>
        public void PlayCard(Player player, EnglishCard card)
        {
            if (State == BasasState.Play && player == CurrPlayer)
            {
                if (GetPlayerStatus(player, out BasasPlayerStatus status))
                {
                    if (!IsValidCard(status, card))
                    {
                        throw new Exception(string.Format("No podés jugar {0}.", card));
                    }

                    HandCards[handPlayerIndex] = card;
                    status.Cards.Remove(card);
                    NextStep();
                }
            }
        }

        /// <summary>
        /// Validates a card.
        /// </summary>
        private bool IsValidCard(BasasPlayerStatus status, EnglishCard card)
        {
            if (status.Cards.Contains(card))
            {
                if (handPlayerIndex > 0 && card.Suit != HandCards[0].Suit)
                {
                    return !status.Cards.Any(c => c.Suit == HandCards[0].Suit);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Continues paused game.
        /// </summary>
        public void Continue()
        {
            if (State == BasasState.HandFinished)
            {
                NextStep();
            }
        }

        /// <summary>
        /// Go to next step of the game.
        /// </summary>
        private void NextStep()
        {
            switch (State)
            {
                case BasasState.Build:
                    StartRound();
                    break;

                case BasasState.Bid:
                    if (!NextPlayer())
                    {
                        State = BasasState.Play;
                    }
                    break;

                case BasasState.Play:
                    if (!NextPlayer())
                    {
                        FinishHand();
                    }
                    break;

                case BasasState.HandFinished:
                    if (leftHandsToPlay > 0)
                    {
                        InitHand();
                        State = BasasState.Play;
                    }
                    else if (currRound < BasasConsts.RoundsNumber)
                    {
                        currRound++;
                        StartRound();
                    }
                    else
                    {
                        State = BasasState.GameFinished;
                    }
                    break;

                default:
                    throw new Exception("Fatal game error!");
            }

            NotifyStateChanged();
        }

        /// <summary>
        /// Go to next player.
        /// </summary>
        private bool NextPlayer()
        {
            if (handPlayerIndex + 1 < PlayersNumber)
            {
                handPlayerIndex++;
                return true;
            }

            handPlayerIndex = 0;
            return false;
        }

        /// <summary>
        /// Start biding in the next round.
        /// </summary>
        private void StartRound()
        {
            currFirstPlayer = GetPlayerIndex(currRound);
            leftHandsToPlay = CurrRoundCards = BasasConsts.RoundCards[currRound];
            InitHand();
            DealCards();
            State = BasasState.Bid;
            Scoreboard.RoundBegins(currRound);
        }

        /// <summary>
        /// Finish the hand and assign points.
        /// </summary>
        private void FinishHand()
        {
            leftHandsToPlay--;

            var index = CalcHandWinner();
            Scoreboard.AddBasaToPlayer(index);
            currFirstPlayer = index;

            State = BasasState.HandFinished;
        }

        /// <summary>
        /// Calculates winner.
        /// </summary>
        private byte CalcHandWinner()
        {
            byte winningIndex = 0;
            EnglishCard winningCard = null;

            for (var index = 0; index < PlayersNumber; index++)
            {
                var playerCard = HandCards[index];

                if (index == 0 || IsNewCardWinner(winningCard, playerCard))
                {
                    winningCard = playerCard;
                    winningIndex = GetPlayerIndex(HandPlayers[index]);
                }
            }

            return winningIndex;
        }

        /// <summary>
        /// Check if a new card is the new winner.
        /// </summary>
        private bool IsNewCardWinner(EnglishCard winningCard, EnglishCard newCard)
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

            foreach (var status in playerStatus)
            {
                status.Cards.Clear();

                for (var ri = 0; ri < CurrRoundCards; ri++)
                {
                    var card = mixedDeck.GetNextCard();
                    status.Cards.Add(card);
                }
            }

            Triunfo = mixedDeck.GetNextCard();
        }

        /// <summary>
        /// Initialize hand players and cards.
        /// </summary>
        private void InitHand()
        {
            for (var index = 0; index < PlayersNumber; index++)
            {
                HandPlayers[index] = GetPlayerStatus(index + currFirstPlayer).Player;
                HandCards[index] = null;
            }

            handPlayerIndex = 0;
        }

        /// <summary>
        /// Get the player status.
        /// </summary>
        private BasasPlayerStatus GetPlayerStatus(int index)
        {
            return playerStatus[GetPlayerIndex(index)];
        }

        /// <summary>
        /// Get the player status.
        /// </summary>
        private bool GetPlayerStatus(Player player, out BasasPlayerStatus status)
        {
            status = playerStatus.FirstOrDefault(i => i.Player == player);
            return status != null;
        }

        /// <summary>
        /// Get a player index.
        /// </summary>
        private byte GetPlayerIndex(Player player)
        {
            GetPlayerStatus(player, out BasasPlayerStatus status);
            return status.Column;
        }

        /// <summary>
        /// Get a player index.
        /// </summary>
        private byte GetPlayerIndex(int index)
        {
            return (byte)(index % PlayersNumber);
        }
    }
}
