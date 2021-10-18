namespace Chess.Models
{
    public class PlayResult : PlayResultEntity
    {
        public PlayResult(string message)
        {
            Message = message;
        }

        public PlayResult()
        {

        }
        public ChessPiece CapturedPiece { get; set; }
        public Location EndLocation { get; set; }
    }
}
