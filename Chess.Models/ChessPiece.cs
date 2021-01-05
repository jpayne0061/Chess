using Chess.Models.Enums;
using System;
using System.Collections.Generic;

namespace Chess.Models
{
    public class ChessPiece
    {
        public readonly Color Color;
        public ChessPiece(int x, int y, Color color)
        {
            CurrentLocation = new Location { X = x, Y = y };
            Color = color;
            IsFirstMove = true;
        }
        public string Name { get; set; }
        public Location CurrentLocation { get; set; }

        public bool IsFirstMove { get; set; }
    }

}
