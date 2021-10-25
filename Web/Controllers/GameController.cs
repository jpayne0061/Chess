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
    public class GameController : ControllerBase
    {
        IMemoryCache _memoryCache;
        IGameSessionDal _gameSessionDal;
        GameFlow _gameFlow;

        public GameController(IMemoryCache memoryCache, 
                            IHubContext<MessageHub> hubContext, 
                            IPlayResultDal playResultDal, 
                            IGameSessionDal gameSessionDal)
        {
            _memoryCache = memoryCache;
            _gameSessionDal = gameSessionDal;
            _gameFlow = new GameFlow(hubContext, playResultDal, gameSessionDal);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<bool> JoinGame(string gameKey)
        {
            GameSession game = _gameSessionDal.GetGameSessionByKey(gameKey);

            if(game == null)
            {
                throw new Exception($"No game could be found by this key: {gameKey}");
            }

            game.Joined = true;

            _gameSessionDal.UpdateGame(game);

            await _gameFlow.SendJoinNotification(game.Key);

            return true;
        }

        [HttpPost]
        public async Task<PawnPromotion> ChoosePawnPromotion([FromBody] PawnPromotion pawnPromotion)
        {
            GameLogic game = (GameLogic)_memoryCache.Get(pawnPromotion.GameKey);

            await _gameFlow.PromotePawn(pawnPromotion, game);

            return pawnPromotion;
        }

        // GET api/values/5
        [HttpGet("{command}")]
        public async Task<PlayResult> MakeMove(string command)
        {
            string[] splitCommand = command.Split(' ');

            string gameId = splitCommand[2];

            GameLogic game = (GameLogic)_memoryCache.Get(gameId);

            return await _gameFlow.MakeMove(command, game);
        }

        // POST api/values
        [HttpGet]
        [Route("[action]")]
        public string CreateGame()
        {
            string gameKey = _gameFlow.CreateGame();

            _memoryCache.Set(gameKey, new GameLogic());

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
                game.DateDisplay = Math.Abs(Math.Floor((DateTime.Now - game.DateStarted.ToLocalTime()).TotalMinutes)).ToString() + " minutes ago";
            }

            return games;
        }
    }
}
