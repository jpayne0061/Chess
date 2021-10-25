using Chess.Models.Enums;

namespace Chess.Models
{
    public class ChessPiece
    {
        protected PieceNames _pieceName;
        public readonly Color Color;
        public ChessPiece(int x, int y, Color color)
        {
            CurrentLocation = new Location { X = x, Y = y };
            Color = color;
            IsFirstMove = true;
        }
        public string Name { get { return _pieceName.ToString(); } }
        public Location CurrentLocation { get; set; }
        public bool IsFirstMove { get; set; }

    }

}
