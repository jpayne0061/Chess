using Chess.Models.Enums;
using System.Collections.Generic;

namespace Chess.Models.Pieces
{
    public class Pawn : ChessPiece
    {
        public Pawn(int x, int y, Color color) : base(x, y, color)
        {
            Name = "Pawn";
        }
    }
}
