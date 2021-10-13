using Chess.Models;
using HotSauceDbOrm;

namespace Web.Data
{
    public class GameKeyDal
    {
        private readonly Executor _executor;

        public GameKeyDal(Executor executor)
        {
            _executor = executor;
        }

        public bool GameKeyExists(string key)
        {
            bool exists = _executor.Read<GameKey>($"select key from gamekey where key = '{key}'").Count > 0;

            return exists;
        }

        public void InsertGameKey(string key)
        {
            _executor.Insert(new GameKey { Key = key });
        }
    }
}
