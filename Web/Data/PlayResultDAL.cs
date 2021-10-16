using Chess.Models;
using HotSauceDbOrm;
using Web.Interfaces;

namespace Web.Data
{ 

    public class PlayResultDal : IPlayResultDal
    {
        private readonly Executor _executor;

        public PlayResultDal(Executor executor)
        {
            _executor = executor;
        }

        public void SavePlayResult(PlayResult playResult)
        {
            try
            {
                var playResultEntity = new PlayResultEntity(playResult);

                _executor.Insert(playResultEntity);
            }
            catch { }
        }
    }
}
