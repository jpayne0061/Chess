using Chess.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Models.Pieces
{
    public class Knight : ChessPiece
    {
        public Knight(int x, int y, Color color) : base(x, y, color)
        {
            Name = "Knight";
        }
    }
}
