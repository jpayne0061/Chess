﻿using Chess.Models;
using Chess.Models.Enums;
using Chess.Models.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuleMaster
{
    public class RuleMaster
    {
        bool _whitesTurn = true;
        HashSet<ChessPiece> _chessPieces;

        public Color GetCurrentPlayer
        {
            get
            {
                return _whitesTurn ? Color.White : Color.Black;
            }
        }


        public RuleMaster()
        {
            var chessPieces = new HashSet<ChessPiece>();

            chessPieces.Add(new Rook(0, 0, Color.White));
            chessPieces.Add(new Knight(1, 0, Color.White));
            chessPieces.Add(new Bishop(2, 0, Color.White));
            chessPieces.Add(new King(3, 0, Color.White));
            chessPieces.Add(new Queen(4, 0, Color.White));
            chessPieces.Add(new Bishop(5, 0, Color.White));
            chessPieces.Add(new Knight(6, 0, Color.White));
            chessPieces.Add(new Rook(7, 0, Color.White));

            chessPieces.Add(new Pawn(0, 1, Color.White));
            chessPieces.Add(new Pawn(1, 1, Color.White));
            chessPieces.Add(new Pawn(2, 1, Color.White));
            chessPieces.Add(new Pawn(3, 1, Color.White));
            chessPieces.Add(new Pawn(4, 1, Color.White));
            chessPieces.Add(new Pawn(5, 1, Color.White));
            chessPieces.Add(new Pawn(6, 1, Color.White));
            chessPieces.Add(new Pawn(7, 1, Color.White));


            chessPieces.Add(new Rook(0, 7, Color.Black));
            chessPieces.Add(new Knight(1, 7, Color.Black));
            chessPieces.Add(new Bishop(2, 7, Color.Black));
            chessPieces.Add(new King(3, 7, Color.Black));
            chessPieces.Add(new Queen(4, 7, Color.Black));
            chessPieces.Add(new Bishop(5, 7, Color.Black));
            chessPieces.Add(new Knight(6, 7, Color.Black));
            chessPieces.Add(new Rook(7, 7, Color.Black));

            chessPieces.Add(new Pawn(0, 6, Color.Black));
            chessPieces.Add(new Pawn(1, 6, Color.Black));
            chessPieces.Add(new Pawn(2, 6, Color.Black));
            chessPieces.Add(new Pawn(3, 6, Color.Black));
            chessPieces.Add(new Pawn(4, 6, Color.Black));
            chessPieces.Add(new Pawn(5, 6, Color.Black));
            chessPieces.Add(new Pawn(6, 6, Color.Black));
            chessPieces.Add(new Pawn(7, 6, Color.Black));

            _chessPieces = chessPieces;
        }

        public PlayResult MakeMove(Location currentLocation, Location proposedLocation)
        {
            ChessPiece chessPiece = _chessPieces.Where(p => p.CurrentLocation.X == currentLocation.X && p.CurrentLocation.Y == currentLocation.Y).FirstOrDefault();

            if(chessPiece == null)
            {
                var nonValidSpace = new PlayResult($"You have not selected a valid move");

                nonValidSpace.PlayValid = false;
                return nonValidSpace;
            }

            if(GetCurrentPlayer != chessPiece.Color)
            {
                var wrongPlayer = new PlayResult($"It is {GetCurrentPlayer}'s turn");

                wrongPlayer.PlayValid = false;
                return wrongPlayer;
            }

            Location oldLocation = new Location(chessPiece.CurrentLocation.X, chessPiece.CurrentLocation.Y);

            bool playerIsInCheckBeforeMove = IsInCheck(GetCurrentPlayer).Any();

            var availableLocations = GetAvailableMoves(chessPiece);

            if(chessPiece is King)
            {
                availableLocations.RemoveWhere(l => PositionCanBeCapturedBy(l, chessPiece.Color).Any());
            }

            if(!availableLocations.Contains(proposedLocation))
            {
                return new PlayResult($"{chessPiece.Name} cannot move to this square");
            }

            var playResult = new PlayResult();

            playResult.CapturedPiece = _chessPieces.Where(c => c.CurrentLocation.X == proposedLocation.X &&
                                c.CurrentLocation.Y == proposedLocation.Y).FirstOrDefault();

            if(playResult.CapturedPiece != null)
            {
                _chessPieces.RemoveWhere(c => c.CurrentLocation.X == proposedLocation.X &&
                                c.CurrentLocation.Y == proposedLocation.Y);
            }

            chessPiece.CurrentLocation = proposedLocation;

            bool playerIsInCheckAfterMove = IsInCheck(GetCurrentPlayer).Any();

            if (playerIsInCheckAfterMove)
            {
                var message = playerIsInCheckBeforeMove ? $"{chessPiece.Color} would still be in check" : $"{chessPiece.Color} would be in check";

                playResult.PlayValid = false;
                playResult.Message = message;
                chessPiece.CurrentLocation = oldLocation;

                return playResult;
            }

            playResult.PlayValid = true;

            if(playResult.PlayValid)
            {
                chessPiece.IsFirstMove = false;
                _whitesTurn = !_whitesTurn;
                playResult.Turn = _whitesTurn ? Color.White : Color.Black;

                var piecesThatCanCaptureKing = IsInCheck(GetCurrentPlayer);

                playResult.IsCheck = piecesThatCanCaptureKing.Any();
                playResult.IsCheckMate = playResult.IsCheck ? IsCheckMate(GetCurrentPlayer, piecesThatCanCaptureKing): false;


            }

            return playResult;
        }

        public HashSet<ChessPiece> IsInCheck(Color color)
        {
            ChessPiece chessPiece = _chessPieces.Where(c => c is King && c.Color == color).FirstOrDefault();

            Location l = chessPiece.CurrentLocation;

            return PositionCanBeCapturedBy(l, chessPiece.Color);
        }

        bool IsCheckMate(Color color, HashSet<ChessPiece> piecesThatCanCaptureKing)
        {
            King king = (King)_chessPieces.Where(c => c is King && c.Color == color).FirstOrDefault();

            if(KingCanEscape(king))
            {
                return false;
            }

            if(piecesThatCanCaptureKing.Count > 1)
            {
                return true;
            }

            ChessPiece pieceThatCanCaptureKing = piecesThatCanCaptureKing.First();

            if(PositionCanBeCapturedBy(pieceThatCanCaptureKing.CurrentLocation, pieceThatCanCaptureKing.Color).Any())
            {
                return false;
            }

            return true;
        }

        bool KingCanEscape(King king)
        {
            HashSet<Location> availableMoves = GetAvailableMoves(king);

            foreach (Location location in availableMoves)
            {
                if (!PositionCanBeCapturedBy(location, king.Color).Any())
                {
                    return true;
                }
            }

            return false;
        }

        bool LocationIsEmpty(int x, int y)
        {
            return !_chessPieces.Where(p => p.CurrentLocation.X == x && p.CurrentLocation.Y == y).Any();
        }

        HashSet<ChessPiece> PositionCanBeCapturedBy(Location location, Color color)
        {
            HashSet<ChessPiece> pieces = new HashSet<ChessPiece>();

            foreach (KeyValuePair<ChessPiece, HashSet<Location>> kvp in GetAllAvailableLocations(color))
            {
                if(kvp.Value.Contains(location))
                {
                    pieces.Add(kvp.Key);
                }
            }

            return pieces;
        }

        public HashSet<Location> GetAvailableMoves(ChessPiece chessPiece)
        {
            HashSet<Location> locations = new HashSet<Location>();

            if (chessPiece is Pawn)
            {
                locations.UnionWith(GetAvailablePawnLocations(chessPiece));
            }

            if (chessPiece is Knight)
            {
                locations.UnionWith(GetAvailableKnightLocations((Knight)chessPiece));
            }

            if (chessPiece is Rook || chessPiece is Queen || chessPiece is King)
            {
                locations.UnionWith(GetHorizontalLocations(chessPiece));
                locations.UnionWith(GetVerticalLocations(chessPiece));
            }

            if (chessPiece is Bishop || chessPiece is Queen || chessPiece is King)
            {
                locations.UnionWith(GetDiagonalLocations(chessPiece));
            }

            if(chessPiece is King)
            {
                GetCastleMove((King)chessPiece);
            }

            return locations;
        }



        public Dictionary<ChessPiece, HashSet<Location>> GetAllAvailableLocations(Color color)
        {
            var pieceToAttackableLocations = new Dictionary<ChessPiece, HashSet<Location>>();

            foreach (var chp in _chessPieces)
            {
                if(chp.Color != color)
                {
                    pieceToAttackableLocations[chp] = GetAvailableMoves(chp);
                }
            }

            return pieceToAttackableLocations;
        }

        HashSet<Location> GetCastleMove(King king)
        {
            HashSet<Location> locations = new HashSet<Location>();

            if (!king.IsFirstMove)
            {
                return locations;
            }

            //get horizontal +- both sides

            //if rook has not moved, castling is possible


            return locations;
        }

        HashSet<Location> GetAvailableKnightLocations(Knight knight)
        {
            HashSet<Location> locations = new HashSet<Location>();

            for (int y = -2; y < 3; y++)
            {
                for (int x = -2; x < 3; x++)
                {
                    if(Math.Abs(x) == Math.Abs(y) || x == 0 || y == 0)
                    {
                        continue;
                    }

                    if(ProposedLocationIsValid(knight.CurrentLocation.X + x, knight.CurrentLocation.Y + y, knight.Color))
                    {
                        locations.Add(new Location(knight.CurrentLocation.X + x, knight.CurrentLocation.Y + y ));
                    }
                }
            }
          
            return locations;
        }

        HashSet<Location> GetAvailablePawnLocations(ChessPiece chessPiece)
        {
            int direction = chessPiece.Color == Color.White ? 1 : -1;

            HashSet<Location> locations = new HashSet<Location>();

            Location moveUpLocation = new Location(chessPiece.CurrentLocation.X, chessPiece.CurrentLocation.Y + 1 * direction);

            Location moveUpTwoLocations = new Location(chessPiece.CurrentLocation.X, chessPiece.CurrentLocation.Y + 2 * direction );

            Location attackRight = new Location(chessPiece.CurrentLocation.X + 1 * direction, chessPiece.CurrentLocation.Y + 1 * direction );

            Location attackLeft = new Location(chessPiece.CurrentLocation.X - 1 * direction, chessPiece.CurrentLocation.Y + 1 * direction );


            if (LocationIsEmpty(moveUpLocation.X, moveUpLocation.Y) && ProposedLocationIsValid(moveUpLocation.X, moveUpLocation.Y, chessPiece.Color))
            {
                locations.Add(moveUpLocation);
            }

            //need to check  bounds
            if (LocationIsEmpty(moveUpLocation.X, moveUpLocation.Y + 1 * direction) && ProposedLocationIsValid(moveUpLocation.X, moveUpLocation.Y + 1 * direction, chessPiece.Color) && chessPiece.IsFirstMove )
            {
                locations.Add(moveUpTwoLocations);
            }

            if (!LocationIsEmpty(attackRight.X, attackRight.Y) && ProposedLocationIsValid(attackRight.X, attackRight.Y, chessPiece.Color))
            {
                locations.Add(attackRight);
            }

            if (!LocationIsEmpty(attackLeft.X, attackLeft.Y) && ProposedLocationIsValid(attackLeft.X, attackLeft.Y, chessPiece.Color))
            {
                locations.Add(attackLeft);
            }

            return locations;
        }

        HashSet<Location> GetHorizontalLocations(ChessPiece chessPiece)
        {
            HashSet<Location> locations = new HashSet<Location>();

            int x = chessPiece.CurrentLocation.X + 1;

           
            while (ProposedLocationIsValid(x, chessPiece.CurrentLocation.Y, chessPiece.Color))
            {
                locations.Add(new Location( x, chessPiece.CurrentLocation.Y ));

                if(chessPiece is King || !LocationIsEmpty(x, chessPiece.CurrentLocation.Y))
                {
                    break;
                }

                x++;
            }


            x = chessPiece.CurrentLocation.X - 1;

            while (ProposedLocationIsValid(x, chessPiece.CurrentLocation.Y, chessPiece.Color))
            {
                locations.Add(new Location(x, chessPiece.CurrentLocation.Y ));

                if (chessPiece is King || !LocationIsEmpty(x, chessPiece.CurrentLocation.Y))
                {
                    break;
                }

                x--;
            }

            return locations;
        }

        HashSet<Location> GetVerticalLocations(ChessPiece chessPiece)
        {
            HashSet<Location> locations = new HashSet<Location>();

            int y = chessPiece.CurrentLocation.Y + 1 ;

            while (ProposedLocationIsValid(chessPiece.CurrentLocation.X, y, chessPiece.Color))
            {
                locations.Add(new Location( chessPiece.CurrentLocation.X, y ));

                if (chessPiece is King || !LocationIsEmpty(chessPiece.CurrentLocation.X, y))
                {
                    break;
                }

                y++;
            }

            y = chessPiece.CurrentLocation.Y - 1;

            while (ProposedLocationIsValid(chessPiece.CurrentLocation.X, y, chessPiece.Color))
            {
                locations.Add(new Location(chessPiece.CurrentLocation.X, y ));

                if (chessPiece is King || !LocationIsEmpty(chessPiece.CurrentLocation.X, y))
                {
                    break;
                }

                y--;
            }

            return locations;
        }

        HashSet<Location> GetDiagonalLocations(ChessPiece chessPiece)
        {
            HashSet<Location> locations = new HashSet<Location>();

            Func<int, int> increment = (input) => input + 1;
            Func<int, int> decrement = (input) => input - 1;

            locations.UnionWith(ProposedLocationsDiagonal(chessPiece, increment, decrement));
            locations.UnionWith(ProposedLocationsDiagonal(chessPiece, increment, increment));
            locations.UnionWith(ProposedLocationsDiagonal(chessPiece, decrement, decrement));
            locations.UnionWith(ProposedLocationsDiagonal(chessPiece, decrement, increment));

            return locations;
        }


        HashSet<Location> ProposedLocationsDiagonal(ChessPiece chessPiece, Func<int, int> funcx, Func<int, int> funcy)
        {
            HashSet<Location> locations = new HashSet<Location>();

            int y = funcy(chessPiece.CurrentLocation.Y);
            int x = funcx(chessPiece.CurrentLocation.X);

            while (ProposedLocationIsValid(x, y, chessPiece.Color))
            {
                locations.Add(new Location(x, y ));

                if (chessPiece is King || !LocationIsEmpty(x, y))
                {
                    break;
                }

                y = funcy(y);
                x = funcx(x);
            }



            return locations;
        }

        bool ProposedLocationIsValid(int x, int y, Color attackingColor)
        {
            bool inBounds = Math.Max(y, x) <= 7 && Math.Min(y, x) >= 0;

            if (!inBounds)
            {
                return false;
            }

            var chp = _chessPieces.Where(c => c.CurrentLocation.X == x && c.CurrentLocation.Y == y).FirstOrDefault();

            return (chp == null) || (chp != null && chp.Color != attackingColor);

        }

    }
}
