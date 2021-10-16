using Chess.Models;
using HotSauceDbOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Interfaces;

namespace Web.Data
{
    public class GameSessionDal : IGameSessionDal
    {
        private readonly Executor _executor;

        public GameSessionDal(Executor executor)
        {
            _executor = executor;
        }

        public bool GameKeyExists(string key)
        {
            bool exists = _executor.Read<GameSession>($"select key from GameSession where key = '{key}'").Count > 0;

            return exists;
        }

        public GameSession GetGameSessionByKey(string key)
        {
            GameSession gameSession = _executor.Read<GameSession>($"select * from GameSession where key = '{key}'").FirstOrDefault();

            return gameSession;
        }

        public void InsertGame(GameSession game)
        {
            _executor.Insert(game);
        }

        public void UpdateGame(GameSession game)
        {
            _executor.Update(game);
        }

        public List<GameSession> GetRecentGames()
        {
            DateTime dateTime = DateTime.Now.AddMinutes(-60);

            string query = $@"select key, DateStarted, DateDisplay from 
                            GameSession where DateStarted > '{dateTime.ToString()}' AND Joined = false";

            List<GameSession> games = _executor.Read<GameSession>(query);

            return games;
        }
    }
}
