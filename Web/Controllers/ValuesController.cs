using System.Collections.Generic;
using System.Threading.Tasks;
using Chess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using ChessGame;
using Web.Services;
using Web.Interfaces;
using System;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        IMemoryCache _memoryCache;
        IGameSessionDal _gameSessionDal;
        GameFlow _gameFlow;

        public ValuesController(IMemoryCache memoryCache, 
                            IHubContext<ChatHub> hubContext, 
                            IPlayResultDal playResultDal, 
                            IGameSessionDal gameSessionDal)
        {
            _memoryCache = memoryCache;
            _gameSessionDal = gameSessionDal;
            _gameFlow = new GameFlow(hubContext, playResultDal, gameSessionDal);
        }

        [HttpGet]
        [Route("[action]")]
        public bool IsValidGame(string gameKey)
        {
            return _gameSessionDal.GameKeyExists(gameKey);
        }

        // GET api/values/5
        [HttpGet("{command}")]
        public async Task<string> MakeMove(string command)
        {
            string[] splitCommand = command.Split(' ');

            string gameId = splitCommand[2];

            Game ruleMaster = (Game)_memoryCache.Get(gameId);

            return await _gameFlow.MakeMove(command, ruleMaster);
        }


        // POST api/values
        [HttpGet]
        [Route("[action]")]
        public string CreateGame()
        {
            string gameKey = _gameFlow.CreateGame();

            _memoryCache.Set(gameKey, new Game());

            return gameKey;
        }

        // POST api/values
        [HttpGet]
        [Route("[action]")]
        public List<GameSession> GetRecentGames()
        {
            List<GameSession> games = _gameSessionDal.GetRecentGames();

            foreach (var game in games)
            {
                game.DateDisplay = Math.Abs(Math.Floor((DateTime.Now - game.DateStarted).TotalMinutes)).ToString() + " minutes ago";
            }

            return games;
        }
    }
}
