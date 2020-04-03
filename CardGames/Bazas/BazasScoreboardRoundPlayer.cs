namespace CardGames.Bazas
{
    public class BazasScoreboardRoundPlayer
    {
        public byte? Bid { get; set; }

        public byte? Bazas { get; set; }

        public byte? Score { get; set; }

        public override string ToString()
        {
            return $"{Bid} / {Bazas}";
        }
    }
}
