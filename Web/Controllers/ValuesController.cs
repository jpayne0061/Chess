using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Web.Data;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        IMemoryCache _memoryCache;
        IHubContext<ChatHub> _hubContext;
        PlayResultDAL _dal;
        private GameKeyDal _gameKeyDal;

        public ValuesController(IMemoryCache memoryCache, IHubContext<ChatHub> hubContext, PlayResultDAL dal, GameKeyDal gameKeyDal)
        {
            _memoryCache = memoryCache;
            _hubContext = hubContext;
            _dal = dal;
            _gameKeyDal = gameKeyDal;
        }

        [HttpGet("{gameKey}")]
        [Route("[action]")]
        public bool ValidGame(string gameKey)
        {
            return _gameKeyDal.GameKeyExists(gameKey);
        }

        // GET api/values/5
        [HttpGet("{command}")]
        public async Task<string> MakeMove(string command)
        {
            string[] splitCommand = command.Split(' ');

            string gameId = splitCommand[2];

            RuleMaster.RuleMaster ruleMaster = (RuleMaster.RuleMaster)_memoryCache.Get(gameId);

            int fromY = int.Parse(splitCommand[0][0].ToString());
            int fromX = int.Parse(splitCommand[0][1].ToString());

            int toY = int.Parse(splitCommand[1][0].ToString());
            int toX = int.Parse(splitCommand[1][1].ToString());

            PlayResult pr = ruleMaster.MakeMove(new Location(fromX, fromY), new Location(toX, toY));

            if(pr.PlayValid)
            {
                pr.Command = command;
                await _hubContext.Clients.All.SendAsync(gameId, pr);
            }

            _dal.SavePlayResult(pr);

            return JsonConvert.SerializeObject(pr);
        }


        // POST api/values
        [HttpGet]
        [Route("[action]")]
        public string SetGame()
        {
            string gameKey = GenerateGameKey();

            while(_gameKeyDal.GameKeyExists(gameKey))
            {
                gameKey = GenerateGameKey();
            }

            _gameKeyDal.InsertGameKey(gameKey);

            _memoryCache.Set(gameKey, new RuleMaster.RuleMaster());

            return gameKey;
        }

        private string GenerateGameKey()
        {
            Random random = new Random();

            List<string> adjectives = new List<string>
            {
                "silly",
                "quiet",
                "funny",
                "loud",
                "small",
                "big",
                "green",
                "jumpy",
                "slippery",
                "tiny",
                "little"
            };

            List<string> nouns = new List<string>
            {
                "dog",
                "cat",
                "rhino",
                "fish",
                "bird",
                "snake",
                "turtle",
                "dragon",
                "panda",
                "dino"
            };

            var adjective = adjectives[random.Next(0, adjectives.Count)];
            var noun = nouns[random.Next(0, nouns.Count)];

            return adjective + noun + random.Next(0, 100).ToString();
        }

    }
}
