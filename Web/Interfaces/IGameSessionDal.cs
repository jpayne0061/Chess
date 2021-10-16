using Chess.Models;
using System.Collections.Generic;

namespace Web.Interfaces
{
    public interface IGameSessionDal
    {
        bool GameKeyExists(string key);

        void InsertGame(GameSession game);

        List<GameSession> GetRecentGames();
    }
}
