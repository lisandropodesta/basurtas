using Lit.Ui;

namespace CardGames
{
    public class Player : BaseModel
    {
        public string NickName { get => nickName; set => SetProp(ref nickName, value, Change.Visibility); }

        private string nickName;
    }
}
