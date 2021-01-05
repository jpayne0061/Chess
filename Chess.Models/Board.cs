using System.Collections.Generic;

namespace Chess.Models
{
    public class Board
    {
        public ChessPiece[,] Locations = new ChessPiece[8,8];
        public Board()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Locations[j, i] = null;
                }
            }

        }
    }
}
