namespace CardGames.Bazas
{
    public static class BazasConsts
    {
        /// <summary>
        /// Total number of rounds.
        /// </summary>
        public const int RoundsNumber = 14;

        /// <summary>
        /// Number of cards per each player in each round.
        /// </summary>
        public static readonly byte[] RoundCards = new byte[] { 7, 6, 5, 4, 3, 2, 1, 1, 2, 3, 4, 5, 6, 7 };
    }
}
