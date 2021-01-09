using Chess.Models.Enums;

namespace Chess.Models
{
    public class PlayResult
    {
        public PlayResult(string message)
        {
            Message = message;
        }

        public PlayResult()
        {

        }

        public string Message { get; set; }
        public bool PlayValid { get; set; }
        public ChessPiece CapturedPiece { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCheckMate { get; set; }
        public Color Turn { get; set; }
        public string Command { get; set; }
    }
}
