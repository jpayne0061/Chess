using Chess.Models;
using HotSauceDbOrm;
using System;
using System.Collections.Generic;
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

        public void InsertGame(GameSession game)
        {
            _executor.Insert(game);
        }

        public List<GameSession> GetRecentGames()
        {
            DateTime dateTime = DateTime.Now.AddMinutes(-60);

            List<GameSession> games = _executor.Read<GameSession>($"select key, DateStarted, DateDisplay from GameSession where DateStarted > '{dateTime.ToString()}'");

            return games;
        }
    }
}
