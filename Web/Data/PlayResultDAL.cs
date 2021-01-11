using Chess.Models;
using SharpDbOrm;

namespace Web.Data
{ 

    public class PlayResultDAL
    {
        private readonly Executor _executor;

        public PlayResultDAL(Executor executor)
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
            catch
            {

            }
        }
    }
}
