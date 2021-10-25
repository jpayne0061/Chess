using Chess.Models.Enums;

namespace Chess.Models.Pieces
{
    public class Rook : ChessPiece
    {
        public Rook(int x, int y, Color color) : base(x, y, color)
        {
            _pieceName = PieceNames.Rook;
        }
    }
}
