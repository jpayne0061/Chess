namespace Chess.Models
{
    /// <summary>
    /// HotSauceDB doesn't support complex properties, so this class will simply inherit from the 
    /// play result entity in order to get around that limitation
    /// </summary>
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
