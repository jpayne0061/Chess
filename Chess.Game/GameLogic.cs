﻿using Chess.Models;
using Chess.Models.Enums;
using Chess.Models.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Chess.UnitTests")]
namespace ChessGame
{
    public class GameLogic
    {
        bool _whitesTurn = true;
        HashSet<ChessPiece> _chessPieces;

        private Color _currentPlayerColor
        {
            get
            {
                return _whitesTurn ? Color.White : Color.Black;
            }
        }

        private Color _otherPlayerColor
        {
            get
            {
                return _whitesTurn ? Color.Black :  Color.White;
            }
        }

        public GameLogic()
        {
            var chessPieces = new HashSet<ChessPiece>();

            chessPieces.Add(new Rook  (0, 0, Color.White));
            chessPieces.Add(new Knight(1, 0, Color.White));
            chessPieces.Add(new Bishop(2, 0, Color.White));
            chessPieces.Add(new King  (3, 0, Color.White));
            chessPieces.Add(new Queen (4, 0, Color.White));
            chessPieces.Add(new Bishop(5, 0, Color.White));
            chessPieces.Add(new Knight(6, 0, Color.White));
            chessPieces.Add(new Rook  (7, 0, Color.White));

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
            chessPieces.Add(new King  (3, 7, Color.Black));
            chessPieces.Add(new Queen (4, 7, Color.Black));
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

            _chessPieces = chessPieces;
        }

        
        internal void OverridePieces(HashSet<ChessPiece> chessPieces)
        {
            _chessPieces = chessPieces;
        }

        public PlayResult MakeMove(Location currentLocation, Location proposedLocation)
        {
            ChessPiece chessPiece = _chessPieces.Where(p => p.CurrentLocation.Equals(currentLocation)).FirstOrDefault();

            if (chessPiece == null)
            {
                var nonValidSpace = new PlayResult($"You have not selected a valid move");

                nonValidSpace.PlayValid = false;
                return nonValidSpace;
            }

            if(_currentPlayerColor != chessPiece.Color)
            {
                var wrongPlayer = new PlayResult($"It is {_currentPlayerColor}'s turn");

                wrongPlayer.PlayValid = false;
                return wrongPlayer;
            }

            Location oldLocation = new Location(chessPiece.CurrentLocation.X, chessPiece.CurrentLocation.Y);

            bool playerIsInCheckBeforeMove = PiecesThatCanCaptureKing(_currentPlayerColor).Any();

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

            playResult.CapturedPiece = _chessPieces.Where(c => c.CurrentLocation.Equals(proposedLocation)).FirstOrDefault();


            if (playResult.CapturedPiece != null)
            {
                _chessPieces.RemoveWhere(c => c.CurrentLocation.Equals(proposedLocation));
            }
            chessPiece.CurrentLocation = proposedLocation;

            bool playerIsInCheckAfterMove = PiecesThatCanCaptureKing(_currentPlayerColor).Any();

            if (playerIsInCheckAfterMove)
            {
                var message = playerIsInCheckBeforeMove ? $"{chessPiece.Color} would still be in check" : $"{chessPiece.Color} would be in check";

                playResult.PlayValid = false;
                playResult.Message = message;
                chessPiece.CurrentLocation = oldLocation;

                return playResult;
            }

            playResult.PlayValid = true;
            playResult.EndLocation = proposedLocation;


            var piecesThatCanCaptureKing = PiecesThatCanCaptureKing(_otherPlayerColor);
            playResult.IsCheck = piecesThatCanCaptureKing.Any();
            playResult.IsCheckMate = playResult.IsCheck ? IsCheckMate(_otherPlayerColor, piecesThatCanCaptureKing) : false;
            chessPiece.IsFirstMove = false;

            if (playResult.PlayValid && IsEligibleForPawnPromotion(chessPiece, proposedLocation))
            {
                playResult.IsEligibleForPawnPromotion = true;
                playResult.Turn = _whitesTurn ? (int)Color.White : (int)Color.Black;

            }
            else if (playResult.PlayValid)
            {
                _whitesTurn = !_whitesTurn;
                playResult.Turn = _whitesTurn ? (int)Color.White : (int)Color.Black;
            }

            return playResult;
        }

        public void PromotePawn(PawnPromotion pawnPromotion)
        {
            ChessPiece chessPiece;

            Color color = _whitesTurn ? Color.White : Color.Black;

            Pawn pawn = (Pawn)_chessPieces.Where(x => x.Color == color
                && x.CurrentLocation.Equals(pawnPromotion.Location)).Single();

            _chessPieces.Remove(pawn);

            switch (pawnPromotion.PieceName)
            {
                case "Rook":
                    chessPiece = new Rook(pawnPromotion.Location.X, pawnPromotion.Location.Y, color);
                    break;
                case "Knight":
                    chessPiece = new Knight(pawnPromotion.Location.X, pawnPromotion.Location.Y, color);
                    break;
                case "Bishop":
                    chessPiece = new Bishop(pawnPromotion.Location.X, pawnPromotion.Location.Y, color);
                    break;
                case "Queen":
                    chessPiece = new Queen(pawnPromotion.Location.X, pawnPromotion.Location.Y, color);
                    break;
                default:
                    chessPiece = new Queen(pawnPromotion.Location.X, pawnPromotion.Location.Y, color);
                    break;
            }

            _chessPieces.Add(chessPiece);

            _whitesTurn = !_whitesTurn;

            pawnPromotion.Turn = _whitesTurn ? 1 : 0;
        }

        private bool IsEligibleForPawnPromotion(ChessPiece chessPiece, Location proposedLocation)
        {
            if(chessPiece is Pawn && chessPiece.Color == Color.Black && proposedLocation.Y == 0)
            {
                return true;
            }

            if (chessPiece is Pawn && chessPiece.Color == Color.White && proposedLocation.Y == 7)
            {
                return true;
            }

            return false;
        }

        private HashSet<ChessPiece> PiecesThatCanCaptureKing(Color color)
        {
            ChessPiece king = _chessPieces.Where(c => c is King && c.Color == color).FirstOrDefault();

            Location kingLocation = king.CurrentLocation;

            return PositionCanBeCapturedBy(kingLocation, king.Color);
        }

        private bool IsCheckMate(Color color, HashSet<ChessPiece> piecesThatCanCaptureKing)
        {
            King king = (King)_chessPieces.Where(c => c is King && c.Color == color).FirstOrDefault();

            IEnumerable<ChessPiece> kingsColorPieces = _chessPieces.Where(c => c.Color == color);

            Color opposingColor = Color.White == color ? Color.Black : Color.White;

            HashSet<Location> availableMoves = GetAllAvailableLocations(opposingColor)
                .Where(x => x.Key.Name != PieceNames.King.ToString()).SelectMany(x => x.Value).ToHashSet();

            if (KingCanEscape(king))
            {
                return false;
            }

            if(piecesThatCanCaptureKing.Count > 1)
            {
                return true;
            }

            ChessPiece pieceThatCanCaptureKing = piecesThatCanCaptureKing.First();

            HashSet<ChessPiece> piecesThatCanCaptureCheckingPiece = PositionCanBeCapturedBy(pieceThatCanCaptureKing.CurrentLocation, pieceThatCanCaptureKing.Color);

            if (piecesThatCanCaptureCheckingPiece.Any())
            {
                foreach (var piece in piecesThatCanCaptureCheckingPiece)
                {
                    if(piece is King)
                    {
                        Location currentLocation = piece.CurrentLocation;

                        //ensure king is not in check if capturing checking piece
                        king.CurrentLocation = pieceThatCanCaptureKing.CurrentLocation;

                        if (PiecesThatCanCaptureKing(_otherPlayerColor).Any())
                        {
                            king.CurrentLocation = currentLocation;
                        }
                        else
                        {
                            king.CurrentLocation = currentLocation;

                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if(pieceThatCanCaptureKing is Knight || pieceThatCanCaptureKing is Pawn)
            {
                //path of knight or pawn cannot be intercepted
                return true;
            }

            if(pieceThatCanCaptureKing is Rook)
            {
                //get path between rook current location and king current location
                HashSet<Location> rookCapturePath = HorizontalVerticalCapturePath(king.CurrentLocation, pieceThatCanCaptureKing.CurrentLocation);

                //can it be intercepted? get available locations of kings color. Do any intersect with capture path? 
                bool capturePathCanBeIntercepted = availableMoves.Intersect(rookCapturePath).Any();

                return !capturePathCanBeIntercepted;

            }
            else if(pieceThatCanCaptureKing is Bishop)
            {
                HashSet<Location> bishopCapturePath = BuildDiagonalPath(king.CurrentLocation, pieceThatCanCaptureKing.CurrentLocation);

                bool capturePathCanBeIntercepted = availableMoves.Intersect(bishopCapturePath).Any();

                return !capturePathCanBeIntercepted;
            }
            else if(pieceThatCanCaptureKing is Queen)
            {
                //at this point, the code doesn't know if the queeen can capture the king
                //using vertical/horizontal moves or diagonal moves

                HashSet<Location> horizontalVerticalCapturePath = HorizontalVerticalCapturePath(king.CurrentLocation, pieceThatCanCaptureKing.CurrentLocation);

                if(!horizontalVerticalCapturePath.Contains(king.CurrentLocation))
                {
                    horizontalVerticalCapturePath = new HashSet<Location>();
                }

                HashSet<Location> diagonalCapturePath = BuildDiagonalPath(king.CurrentLocation, pieceThatCanCaptureKing.CurrentLocation);

                bool capturePathCanBeIntercepted = horizontalVerticalCapturePath.Union(diagonalCapturePath).Intersect(availableMoves).Any();

                return !capturePathCanBeIntercepted;
            }

            return true;
        }

        HashSet<Location> HorizontalVerticalCapturePath(Location kingLocation, Location rookLocation)
        {
            bool xAxisCommon = kingLocation.X == rookLocation.X;

            HashSet<Location> capturePath;

            if (xAxisCommon)
            {
                capturePath = BuildHorizontalVerticalPath(kingLocation.X, kingLocation.Y, rookLocation.Y);
            }
            else
            {
                capturePath = BuildHorizontalVerticalPath(kingLocation.Y, kingLocation.X, rookLocation.X);
            }

            return capturePath;
        }

        HashSet<Location> BuildHorizontalVerticalPath(int commonAxis, int targetPosition, int capturerPosition)
        {
            HashSet<Location> capturePath = new HashSet<Location>();

            int maxPosition = Math.Max(targetPosition, capturerPosition);

            int minPosition = Math.Min(targetPosition, capturerPosition);

            for (int i = minPosition; i <= maxPosition; i++)
            {
                capturePath.Add(new Location(commonAxis, i));
            }

            return capturePath;
        }

        HashSet<Location> BuildDiagonalPath(Location targetPosition, Location capturerPosition)
        {
            HashSet<Location> capturePath = new HashSet<Location>();

            int slope = 0;

            if(capturerPosition.X - targetPosition.X == 0)
            {
                slope = 0;
            }
            else
            {
                slope = (capturerPosition.Y - targetPosition.Y) / (capturerPosition.X - targetPosition.X);
            }

            var yIntercept = targetPosition.Y - slope * targetPosition.X;

            Location minXLocation = targetPosition.X < capturerPosition.X ? targetPosition : capturerPosition;
            Location maxXLocation = targetPosition.X > capturerPosition.X ? targetPosition : capturerPosition;

            for (int i = minXLocation.X + 1; i < maxXLocation.X; i++)
            {
                var y = slope * i + yIntercept;

                capturePath.Add(new Location(i, y));
            }

            return capturePath;
        }
   

        bool KingCanEscape(King king)
        {
            Location currentLocation = king.CurrentLocation;

            HashSet<Location> availableMoves = GetAvailableMoves(king);

            foreach (Location location in availableMoves)
            {
                if (!PositionCanBeCapturedBy(location, king.Color).Any())
                {
                    king.CurrentLocation = location;

                    if(PiecesThatCanCaptureKing(_otherPlayerColor).Any())
                    {
                        king.CurrentLocation = currentLocation;
                    }
                    else
                    {
                        king.CurrentLocation = currentLocation;

                        return true;
                    }
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

            var chessPiece = _chessPieces.Where(c => c.CurrentLocation.Equals(x, y)).FirstOrDefault();

            return (chessPiece == null) || (chessPiece != null && chessPiece.Color != attackingColor);

        }

    }
}
