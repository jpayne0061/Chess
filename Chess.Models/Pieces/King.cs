using Chess.Models.Enums;

namespace Chess.Models.Pieces
{
    public class King : ChessPiece
    {
        public King(int x, int y, Color color) : base(x, y, color)
        {
            _pieceName = PieceNames.King;
        }
    }
}
