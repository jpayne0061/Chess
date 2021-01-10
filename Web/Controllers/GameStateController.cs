using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameStateController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public GameStateController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet("{gameId}")]
        public bool GameIdIsValid(string gameId)
        {
            var gameIsValid = _memoryCache.TryGetValue(gameId, out object val);

            return gameIsValid;
        }
    }
}