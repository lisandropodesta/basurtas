using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGames.Bazas
{
    /// <summary>
    /// Game of Bazas.
    /// </summary>
    public class BazasGame : CardGame
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
        public BazasState State { get; private set; }

        /// <summary>
        /// Scoreboard.
        /// </summary>
        public BazasScoreboard Scoreboard { get; private set; }

        /// <summary>
        /// Current player.
        /// </summary>
        public Player CurrPlayer => HandPlayers[handPlayerIndex];

        /// <summary>
        /// Hand winned.
        /// </summary>
        public Player HandWinner { get; private set; }

        /// <summary>
        /// Name of the winner(s).
        /// </summary>
        public string GameWinner { get; private set; }

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
        private BazasPlayerStatus[] playerStatus;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BazasGame()
        {
            State = BazasState.Build;
        }

        /// <summary>
        /// Game started.
        /// </summary>
        protected override void StartGame()
        {
            currRound = 0;

            Scoreboard = new BazasScoreboard(PlayersNumber);

            playerStatus = new BazasPlayerStatus[PlayersNumber];
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
            if (State != BazasState.Build)
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
            MixPlayers();

            for (byte column = 0; column < Players.Count; column++)
            {
                playerStatus[column] = new BazasPlayerStatus(column, Players[column]);
            }
        }

        /// <summary>
        /// Get current player cards.
        /// </summary>
        public List<EnglishCard> GetCards(Player player)
        {
            if (GetPlayerStatus(player, out BazasPlayerStatus status))
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
            if (GetPlayerStatus(player, out BazasPlayerStatus status))
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

            if (State == BazasState.Bid && player == CurrPlayer)
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
            if (State == BazasState.Play && player == CurrPlayer)
            {
                if (GetPlayerStatus(player, out BazasPlayerStatus status))
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
        /// Get the current scoreboard for the player.
        /// </summary>
        public BazasScoreboardRoundPlayer GetPlayerScore(Player player)
        {
            var index = GetPlayerIndex(player);
            return Scoreboard.Rounds[currRound].Player[index];
        }

        /// <summary>
        /// Validates a card.
        /// </summary>
        private bool IsValidCard(BazasPlayerStatus status, EnglishCard card)
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
            if (State == BazasState.HandFinished)
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
                case BazasState.Build:
                    StartRound();
                    break;

                case BazasState.Bid:
                    if (!NextPlayer())
                    {
                        State = BazasState.Play;
                    }
                    break;

                case BazasState.Play:
                    if (!NextPlayer())
                    {
                        FinishHand();
                    }
                    break;

                case BazasState.HandFinished:
                    if (leftHandsToPlay > 0)
                    {
                        InitHand();
                        State = BazasState.Play;
                    }
                    else
                    {
                        if (currRound < BazasConsts.RoundsNumber - 1)
                        {
                            currRound++;
                            StartRound();
                        }
                        else
                        {
                            CalcGameWinner();
                            State = BazasState.GameFinished;
                        }
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
            leftHandsToPlay = CurrRoundCards = BazasConsts.RoundCards[currRound];
            InitHand();
            DealCards();
            State = BazasState.Bid;
            Scoreboard.RoundBegins(currRound);
        }

        /// <summary>
        /// Finish the hand and assign points.
        /// </summary>
        private void FinishHand()
        {
            leftHandsToPlay--;

            HandWinner = CalcHandWinner();
            var index = GetPlayerIndex(HandWinner);
            currFirstPlayer = index;

            Scoreboard.HandEnd(index);
            if (leftHandsToPlay == 0)
            {
                Scoreboard.RoundEnds();
            }

            State = BazasState.HandFinished;
        }

        /// <summary>
        /// Calculates the game winner.
        /// </summary>
        private void CalcGameWinner()
        {
            var winner = new List<string>();
            var score = 0;

            for (var index = 0; index < PlayersNumber; index++)
            {
                var status = playerStatus[index];
                var finalScore = Scoreboard.Rounds[BazasConsts.RoundsNumber - 1].Player[status.Column].Score ?? 0;
                if (score < finalScore)
                {
                    winner.Clear();
                    winner.Add(status.Player.NickName);
                    score = finalScore;
                }
                else if (score == finalScore)
                {
                    winner.Add(status.Player.NickName);
                }
            }

            GameWinner = string.Join(", ", winner);
        }

        /// <summary>
        /// Calculates winner.
        /// </summary>
        private Player CalcHandWinner()
        {
            EnglishCard winningCard = null;
            Player winner = null;

            for (var index = 0; index < PlayersNumber; index++)
            {
                var playerCard = HandCards[index];

                if (index == 0 || IsNewCardWinner(winningCard, playerCard))
                {
                    winningCard = playerCard;
                    winner = HandPlayers[index];
                }
            }

            return winner;
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
        private BazasPlayerStatus GetPlayerStatus(int index)
        {
            return playerStatus[GetPlayerIndex(index)];
        }

        /// <summary>
        /// Get the player status.
        /// </summary>
        private bool GetPlayerStatus(Player player, out BazasPlayerStatus status)
        {
            status = playerStatus.FirstOrDefault(i => i.Player == player);
            return status != null;
        }

        /// <summary>
        /// Get a player index.
        /// </summary>
        private byte GetPlayerIndex(Player player)
        {
            GetPlayerStatus(player, out BazasPlayerStatus status);
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
