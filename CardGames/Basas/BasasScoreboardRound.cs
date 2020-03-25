namespace CardGames.Basas
{
    public class BasasScoreboardRound
    {
        public BasasScoreboardRoundPlayer[] Player { get; private set; }

        public BasasScoreboardRound(int playersCount)
        {
            Player = new BasasScoreboardRoundPlayer[playersCount];

            for (var i = 0; i < playersCount; i++)
            {
                Player[i] = new BasasScoreboardRoundPlayer();
            }
        }
    }
}
