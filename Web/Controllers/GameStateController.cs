using Microsoft.AspNetCore.Mvc;
using Web.Data;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameStateController : ControllerBase
    {
        private readonly GameSessionDal _gameKeyDal;

        public GameStateController(GameSessionDal gameKeyDal)
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