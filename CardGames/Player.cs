using Lit.Ui;

namespace CardGames
{
    public class Player : BaseModel
    {
        public bool IsConnected { get => isConnected; set => SetProp(ref isConnected, value, Change.Visibility); }

        private bool isConnected;

        public string NickName { get => nickName; set => SetProp(ref nickName, value, Change.Visibility); }

        private string nickName;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Player()
        {
            isConnected = true;
        }

        public override string ToString()
        {
            return NickName;
        }
    }
}
