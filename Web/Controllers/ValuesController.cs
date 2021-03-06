﻿using System.Threading.Tasks;
using Chess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SharpDbOrm;
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
        Executor _executor;

        public ValuesController(IMemoryCache memoryCache, IHubContext<ChatHub> hubContext, PlayResultDAL dal)
        {
            _memoryCache = memoryCache;
            _hubContext = hubContext;
            _dal = dal;
        }

        // GET api/values
        [HttpGet]
        public void Get()
        {

        }

        [HttpGet("{gameId}")]
        [Route("[action]")]
        public bool ValidGame(string gameId)
        {
            object val;

            return _memoryCache.TryGetValue(gameId, out val);
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
        [HttpPost]
        public void SetGame([FromBody] string gameId)
        {
            RuleMaster.RuleMaster ruleMaster = new RuleMaster.RuleMaster();

            _memoryCache.Set(gameId, ruleMaster);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
    }
}
