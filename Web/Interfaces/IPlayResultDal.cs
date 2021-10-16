using Chess.Models;
namespace Web.Interfaces
{
    public interface IPlayResultDal
    {
        void SavePlayResult(PlayResult playResult);
    }
}
