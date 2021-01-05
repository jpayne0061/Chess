using System;
using System.Collections.Generic;
using Chess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        IMemoryCache _memoryCache;

        public ValuesController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            List<ChessPiece> capturedPieces = new List<ChessPiece>();

            Board board = new Board();

            RuleMaster.RuleMaster ruleMaster = new RuleMaster.RuleMaster();

            _memoryCache.Set("1", ruleMaster);

            return new string[] { };
        }

        // GET api/values/5s
        [HttpGet("{id}")]
        public string Get(string id)
        {
            RuleMaster.RuleMaster ruleMaster = (RuleMaster.RuleMaster)_memoryCache.Get("1");

            string[] splitCommand = id.Split(' ');

            int fromY = int.Parse(splitCommand[0][0].ToString());
            int fromX = int.Parse(splitCommand[0][1].ToString());

            int toY = int.Parse(splitCommand[1][0].ToString());
            int toX = int.Parse(splitCommand[1][1].ToString());

            PlayResult pr = ruleMaster.MakeMove(new Location(fromX, fromY), new Location(toX, toY));

            return JsonConvert.SerializeObject(pr);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
