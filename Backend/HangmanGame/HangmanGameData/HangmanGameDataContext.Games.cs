using System.Data.Linq;

namespace HangmanGameData
{
    public partial class HangmanGameDataContext
    {
        public Table<Categories> Categories => GetTable<Categories>();
        public Table<Words> Words => GetTable<Words>();
        public Table<Games> Games => GetTable<Games>();
        public Table<GameMoves> GameMoves => GetTable<GameMoves>();
    }
}
