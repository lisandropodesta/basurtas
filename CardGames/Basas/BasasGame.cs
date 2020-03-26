using System;
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
        /// Last player flag.
        /// </summary>
        protected bool IsLastPlayer => currFirstPlayer == GetNextPlayerIndex();

        private Player[] playerPositions;

        private byte currentRound;

        private byte currPlayerIndex;

        private byte currFirstPlayer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BasasGame()
        {
            State = BasasState.Build;
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

            playerPositions = Players.ToArray();
            // TODO: mix players

            currentRound = 0;
            currFirstPlayer = 0;
            currPlayerIndex = 0;

            NextStep(false);
            NotifyStateChanged();
        }

        /// <summary>
        /// Go to next game step.
        /// </summary>
        private void NextStep(bool skip)
        {
            if (skip)
            {
                currPlayerIndex = (byte)((currPlayerIndex + 1) % PlayersNumber);
            }

            CurrentPlayer = playerPositions[currPlayerIndex];

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
        /// Get the next player of the round.
        /// </summary>
        private byte GetNextPlayerIndex()
        {
            return (byte)((currPlayerIndex + 1) % PlayersNumber);
        }

        /// <summary>
        /// Start biding in the next round.
        /// </summary>
        private void StartNextRound()
        {
            currentRound++;
            State = BasasState.Bid;
            Scoreboard.RoundBegins(currentRound);
        }
    }
}
