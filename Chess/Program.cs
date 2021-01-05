using Chess.Models;
using Chess.Models.Enums;
using Chess.Models.Pieces;
using RuleMaster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ChessPiece> capturedPieces = new List<ChessPiece>();

            Board board = new Board();

            HashSet<ChessPiece> chessPieces = SetupPieces();

            RuleMaster.RuleMaster ruleMaster = new RuleMaster.RuleMaster();

            BuildGrid(chessPieces);

            while (true)
            {
                string rawCommand = Console.ReadLine();

                string[] splitCommand = rawCommand.Split(' ');

                int fromY = int.Parse(splitCommand[0][0].ToString());
                int fromX = int.Parse(splitCommand[0][1].ToString());

                int toY = int.Parse(splitCommand[1][0].ToString());
                int toX = int.Parse(splitCommand[1][1].ToString());

                //bool currentPlayerIsInCheck = ruleMaster.IsInCheck();

                //if(currentPlayerIsInCheck)
                //{
                //    Console.WriteLine($"{ruleMaster.GetCurrentPlayer.ToString()} is in check");
                //}

                //ChessPiece chp = chessPieces.Where(p => p.CurrentLocation.X == fromX && p.CurrentLocation.Y == fromY).First();

                //PlayResult pr = ruleMaster.MakeMove(chp, new Location(toX, toY));

                //BuildGrid(chessPieces);

                //Console.ForegroundColor = ConsoleColor.Red;

                //Console.WriteLine(pr.Message);
            }

        }

        public static HashSet<ChessPiece> SetupPieces()
        {
            var chessPieces = new HashSet<ChessPiece>();

            chessPieces.Add(new Rook  (0, 0,Color.White));
            chessPieces.Add(new Knight(1, 0,Color.White));
            chessPieces.Add(new Bishop(2, 0,Color.White));
            chessPieces.Add(new Queen (3, 0,Color.White));
            chessPieces.Add(new King  (4, 0,Color.White));
            chessPieces.Add(new Bishop(5, 0,Color.White));
            chessPieces.Add(new Knight(6, 0,Color.White));
            chessPieces.Add(new Rook  (7, 0,Color.White));

            chessPieces.Add(new Pawn(0, 1, Color.White));
            chessPieces.Add(new Pawn(1, 1, Color.White));
            chessPieces.Add(new Pawn(2, 1, Color.White));
            chessPieces.Add(new Pawn(3, 1, Color.White));
            chessPieces.Add(new Pawn(4, 1, Color.White));
            chessPieces.Add(new Pawn(5, 1, Color.White));
            chessPieces.Add(new Pawn(6, 1, Color.White));
            chessPieces.Add(new Pawn(7, 1, Color.White));


            chessPieces.Add(new Rook  (0, 7, Color.Black));
            chessPieces.Add(new Knight(1, 7, Color.Black));
            chessPieces.Add(new Bishop(2, 7, Color.Black));
            chessPieces.Add(new Queen (3, 7, Color.Black));
            chessPieces.Add(new King  (4, 7, Color.Black));
            chessPieces.Add(new Bishop(5, 7, Color.Black));
            chessPieces.Add(new Knight(6, 7, Color.Black));
            chessPieces.Add(new Rook  (7, 7, Color.Black));

            chessPieces.Add(new Pawn(0, 6, Color.Black));
            chessPieces.Add(new Pawn(1, 6, Color.Black));
            chessPieces.Add(new Pawn(2, 6, Color.Black));
            chessPieces.Add(new Pawn(3, 6, Color.Black));
            chessPieces.Add(new Pawn(4, 6, Color.Black));
            chessPieces.Add(new Pawn(5, 6, Color.Black));
            chessPieces.Add(new Pawn(6, 6, Color.Black));
            chessPieces.Add(new Pawn(7, 6, Color.Black));

            return chessPieces;
        }

        
        static void BuildGrid(HashSet<ChessPiece> chessPieces)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkGray;

            char[][] grid = new char[][] {
                new char[8],
                new char[8],
                new char[8],
                new char[8],
                new char[8],
                new char[8],
                new char[8],
                new char[8]
            };

            foreach (var piece in chessPieces)
            {
                char c = ' ';

                if(piece is Rook)
                {
                    c = 'r';
                }
                if(piece is Knight)
                {
                    c = 'k';
                }
                if (piece is Bishop)
                {
                    c = 'b';
                }
                if (piece is King)
                {
                    c = 'K';
                }
                if (piece is Queen)
                {
                    c = 'Q';
                }
                if (piece is Pawn)
                {
                    c = 'p';
                }



                grid[piece.CurrentLocation.Y][piece.CurrentLocation.X] = c;
            }

            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid.Length; j++)
                {
                    if(grid[i][j] == '\0')
                    {
                        grid[i][j] = ' ';
                    }
                }
            }

            int y = 0;

            foreach (var charArr in grid)
            {

                var line = string.Join('\0', charArr);

                int x = 0;
                foreach (char c in charArr)
                {
                    ChessPiece chp = chessPieces.Where(cx => cx.CurrentLocation.X == x && cx.CurrentLocation.Y == y).FirstOrDefault();

                    if(chp == null)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(c);
                        x++;
                        continue;
                    }

                    if(chp.Color == Color.White)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }


                    if (chp.Color == Color.Black)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.Write(c);
                    x++;
                }

                Console.Write('\n');

                y++;
            }

            //return gridString;
        }

    }
}
