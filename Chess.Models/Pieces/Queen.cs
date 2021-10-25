using Chess.Models.Enums;

namespace Chess.Models.Pieces
{
    public class Queen : ChessPiece
    {
        public Queen(int x, int y, Color color) : base(x, y, color)
        {
            _pieceName = PieceNames.Queen;
        }
    }
}
