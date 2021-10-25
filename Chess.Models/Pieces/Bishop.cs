using Chess.Models.Enums;

namespace Chess.Models.Pieces
{
    public class Bishop : ChessPiece
    {
        public Bishop(int x, int y, Color color) : base(x, y, color)
        {
            _pieceName = PieceNames.Bishop;
        }
    }
}
