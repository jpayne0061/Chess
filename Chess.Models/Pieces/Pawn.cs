using Chess.Models.Enums;

namespace Chess.Models.Pieces
{
    public class Pawn : ChessPiece
    {
        public Pawn(int x, int y, Color color) : base(x, y, color)
        {
            _pieceName = PieceNames.Pawn;
        }
    }
}
