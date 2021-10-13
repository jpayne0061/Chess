using Microsoft.AspNetCore.Mvc;
using Web.Data;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameStateController : ControllerBase
    {
        private readonly GameKeyDal _gameKeyDal;

        public GameStateController(GameKeyDal gameKeyDal)
        {
            _gameKeyDal = gameKeyDal;
        }

        [HttpGet("{gameKey}")]
        public bool GameIdIsValid(string gameKey)
        {
            return _gameKeyDal.GameKeyExists(gameKey);
        }
    }
}