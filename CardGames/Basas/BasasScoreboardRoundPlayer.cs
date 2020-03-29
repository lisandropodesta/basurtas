namespace CardGames.Basas
{
    public class BasasScoreboardRoundPlayer
    {
        public byte? Bid { get; set; }

        public byte? Basas { get; set; }

        public byte? Score { get; set; }

        public override string ToString()
        {
            return $"{Bid} / {Basas}";
        }
    }
}
