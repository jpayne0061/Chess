using Chess.Models;
using Chess.Models.Enums;
using Chess.Models.Pieces;
using ChessGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Chess.UnitTests
{
    [TestClass]
    public class CheckTests
    {
        [TestMethod]
        public void IsCheck_1()
        {
            GameLogic game = new GameLogic();

            var chessPieces = new HashSet<ChessPiece>();

            chessPieces.Add(new Queen(3, 0, Color.Black));
            chessPieces.Add(new King (4, 0, Color.Black));
            chessPieces.Add(new Queen(2, 3, Color.White));
            chessPieces.Add(new King (7, 0, Color.White));

            game.OverridePieces(chessPieces);

            PlayResult pr = game.MakeMove(new Location(2, 3), new Location(2, 2));

            Assert.IsTrue(pr.IsCheck);
        }
    }
}
