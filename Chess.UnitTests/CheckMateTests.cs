using Chess.Models;
using Chess.Models.Enums;
using Chess.Models.Pieces;
using ChessGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Chess.UnitTests
{
    [TestClass]
    public class CheckMateTests
    {
        [TestMethod]
        public void IsCheckMate_1()
        {
            GameLogic game = new GameLogic();

            var chessPieces = new HashSet<ChessPiece>();

            chessPieces.Add(new Rook (0, 7, Color.Black));
            chessPieces.Add(new King (1, 7, Color.Black));
            chessPieces.Add(new Queen(3, 7, Color.Black));
            chessPieces.Add(new Queen(2, 5, Color.White));
            chessPieces.Add(new Rook (1, 0, Color.White));
            chessPieces.Add(new King (7, 0, Color.White));


            game.OverridePieces(chessPieces);

            PlayResult pr = game.MakeMove(new Location(2, 5), new Location(1, 6));

            Assert.IsTrue(pr.IsCheckMate);
        }

        [TestMethod]
        public void IsCheckMate_2()
        {
            GameLogic game = new GameLogic();

            var chessPieces = new HashSet<ChessPiece>();

            chessPieces.Add(new Rook  (7, 7, Color.Black));
            chessPieces.Add(new Pawn  (6, 6, Color.Black));
            chessPieces.Add(new Knight(7, 6, Color.White));
            chessPieces.Add(new King  (4, 7, Color.Black));
            chessPieces.Add(new Rook  (3, 6, Color.White));
            chessPieces.Add(new Bishop(2, 7, Color.Black));
            chessPieces.Add(new Knight(1, 7, Color.Black));

            chessPieces.Add(new Bishop(5, 5, Color.White));
            chessPieces.Add(new Bishop(4, 5, Color.White));
            chessPieces.Add(new King  (0, 0, Color.White));

            game.OverridePieces(chessPieces);

            PlayResult pr = game.MakeMove(new Location(3, 6), new Location(3, 7));

            Assert.IsTrue(pr.IsCheckMate);
        }
    }
}
