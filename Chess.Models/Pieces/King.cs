using Chess.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Models.Pieces
{
    public class King : ChessPiece
    {
        public King(int x, int y, Color color) : base(x, y, color)
        {
            Name = "King";
        }
    }
}
