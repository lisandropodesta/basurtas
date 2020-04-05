using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CardGames
{
    public abstract class CardGame
    {
        /// <summary>
        /// Players.
        /// </summary>
        public IReadOnlyList<Player> Players => players;

        /// <summary>
        /// Players.
        /// </summary>
        public IReadOnlyList<Player> Viewers => viewers;

        /// <summary>
        /// Players number.
        /// </summary>
        public int PlayersNumber => players.Count;

        /// <summary>
        /// Players list as text.
        /// </summary>
        public string PlayersListText { get; private set; }

        /// <summary>
        /// Viewers list as text.
        /// </summary>
        public string ViewersListText { get; private set; }

        /// <summary>
        /// Min players number.
        /// </summary>
        public abstract int MinPlayersNumber { get; }

        /// <summary>
        /// Max players number.
        /// </summary>
        public abstract int MaxPlayersNumber { get; }

        /// <summary>
        /// State changed event.
        /// </summary>
        public event EventHandler OnStateChanged;

        // Players list.
        private readonly List<Player> players = new List<Player>();

        // Viewer list
        private readonly List<Player> viewers = new List<Player>();

        private bool started;

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void Start()
        {
            if (!IsReadyToStart(out string text))
            {
                throw new Exception(text);
            }

            started = true;
            StartGame();
        }

        /// <summary>
        /// Adds a player.
        /// </summary>
        public void AddPlayer(Player player)
        {
            lock (players)
            {
                if (!players.Contains(player))
                {
                    if (IsAllowedToPlay(player, out string reason))
                    {
                        player.PropertyChanged += OnPlayerPropertyChanged;
                        players.Add(player);
                        DoPlayersListChanged();
                    }
                    else if (IsAllowedToSee(player))
                    {
                        player.PropertyChanged += OnViewerPropertyChanged;
                        viewers.Add(player);
                        DoViewersListChanged();
                        throw new Exception($"Solo podés mirar, porque {reason}");
                    }
                    else
                    {
                        throw new Exception(reason);
                    }
                }
            }
        }

        /// <summary>
        /// Removes a player.
        /// </summary>
        public void RemovePlayer(Player player)
        {
            try
            {
                lock (players)
                {
                    if (players.Contains(player))
                    {
                        player.PropertyChanged -= OnPlayerPropertyChanged;
                        players.Remove(player);
                        DoPlayersListChanged();
                    }
                    else if (viewers.Contains(player))
                    {
                        player.PropertyChanged -= OnViewerPropertyChanged;
                        viewers.Remove(player);
                        DoViewersListChanged();
                    }
                }
            }
            catch (Exception x)
            {
            }

            // TODO: check what happens if the game has started.
        }

        /// <summary>
        /// Mix the players list.
        /// </summary>
        public void MixPlayers()
        {
            Tool.Mix(players);
        }

        #region Virtual abstract members

        /// <summary>
        /// Check if the player is allowed to enter.
        /// </summary>
        protected virtual bool IsAllowedToPlay(Player player, out string reason)
        {
            if (PlayersNumber >= MaxPlayersNumber)
            {
                reason = "Máxima cantidad de jugadores alcanzada";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        /// <summary>
        /// Check if the player is allowed to see.
        /// </summary>
        protected virtual bool IsAllowedToSee(Player player)
        {
            return true;
        }

        /// <summary>
        /// Game started.
        /// </summary>
        protected abstract void StartGame();

        #endregion

        /// <summary>
        /// Check if the game is ready to start.
        /// </summary>
        protected bool IsReadyToStart(out string text)
        {
            if (started)
            {
                text = "Este juego ya inició.";
                return false;
            }

            if (PlayersNumber < MinPlayersNumber)
            {
                text = "No se alcanzó el número mínimo de jugadores.";
                return false;
            }

            text = string.Empty;
            return true;
        }

        protected void NotifyStateChanged()
        {
            DoStateChanged();
        }

        private void OnPlayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DoPlayersListChanged();
        }

        private void OnViewerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DoViewersListChanged();
        }

        private void DoPlayersListChanged()
        {
            PlayersListText = string.Join(", ", players.Select(i => i.NickName));
            DoStateChanged();
        }

        private void DoViewersListChanged()
        {
            ViewersListText = string.Join(", ", viewers.Select(i => i.NickName));
            DoStateChanged();
        }

        private void DoStateChanged()
        {
            OnStateChanged?.Invoke(this, null);
        }
    }
}
