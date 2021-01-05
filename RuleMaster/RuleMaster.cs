using Chess.Models;
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

            bool playerIsInCheckBeforeMove = IsInCheck(GetCurrentPlayer);

            var availableLocations = GetAvailableLocations(chessPiece);

            if(chessPiece is King)
            {
                availableLocations.RemoveWhere(l => PositionCanBeAttacked(l, chessPiece.Color));
            }

            if(!availableLocations.Where(l => l.X == proposedLocation.X && l.Y == proposedLocation.Y).Any())
            {
                return new PlayResult($"{chessPiece.Name} cannot move to this square");
            }

            var playResult = new PlayResult($"{chessPiece.Name} has been moved");

            playResult.CapturedPiece = _chessPieces.Where(c => c.CurrentLocation.X == proposedLocation.X &&
                                c.CurrentLocation.Y == proposedLocation.Y).FirstOrDefault();

            if(playResult.CapturedPiece != null)
            {
                _chessPieces.RemoveWhere(c => c.CurrentLocation.X == proposedLocation.X &&
                                c.CurrentLocation.Y == proposedLocation.Y);
            }

            chessPiece.CurrentLocation = proposedLocation;

            bool playerIsInCheckAfterMove = IsInCheck(GetCurrentPlayer);

            if(playerIsInCheckBeforeMove && playerIsInCheckAfterMove)
            {
                playResult.PlayValid = false;
                playResult.Message = $"{chessPiece.Color} would still be in check";
                chessPiece.CurrentLocation = oldLocation;

                return playResult;
            }

            if(!playerIsInCheckBeforeMove && playerIsInCheckAfterMove)
            {
                playResult.PlayValid = false;
                playResult.Message = $"{chessPiece.Color} would be in check";
                chessPiece.CurrentLocation = oldLocation;

                return playResult;
            }

            playResult.PlayValid = true;

            if(playResult.PlayValid)
            {
                chessPiece.IsFirstMove = false;
                _whitesTurn = !_whitesTurn;
                playResult.Turn = _whitesTurn ? Color.White : Color.Black;
                playResult.IsCheck = IsInCheck(GetCurrentPlayer);
            }

            return playResult;
        }

        public bool IsInCheck(Color color)
        {
            ChessPiece chessPiece = _chessPieces.Where(c => c is King && c.Color == color).FirstOrDefault();

            Location l = chessPiece.CurrentLocation;

            return PositionCanBeAttacked(l, chessPiece.Color);
        }

        bool IsCheckMate()
        {
            throw new NotImplementedException();
        }

        bool LocationIsEmpty(int x, int y)
        {
            return !_chessPieces.Where(p => p.CurrentLocation.X == x && p.CurrentLocation.Y == y).Any();
        }

        bool PositionCanBeAttacked(Location location, Color color)
        {
            return GetAllAvailableLocations(color).
                Where(c => c.X == location.X && c.Y == location.Y).FirstOrDefault() != null;
        }

        public HashSet<Location> GetAvailableLocations(ChessPiece chessPiece)
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

            return locations;
        }

        public HashSet<Location> GetAllAvailableLocations(Color color)
        {
            HashSet<Location> locations = new HashSet<Location>();

            foreach (var chp in _chessPieces)
            {
                if(chp.Color != color)
                {
                    locations.UnionWith(GetAvailableLocations(chp));
                }
            }

            return locations;
        }

        HashSet<Location> GetAvailableKnightLocations(Knight knight)
        {
            HashSet<Location> locations = new HashSet<Location>();

            if(ProposedLocationIsValid(knight.CurrentLocation.X + 1, knight.CurrentLocation.Y + 2, knight.Color))
            {
                locations.Add(new Location { X = knight.CurrentLocation.X + 1, Y = knight.CurrentLocation.Y + 2 });
            }

            if (ProposedLocationIsValid(knight.CurrentLocation.X + 2, knight.CurrentLocation.Y + 1, knight.Color))
            {
                locations.Add(new Location { X = knight.CurrentLocation.X + 2, Y = knight.CurrentLocation.Y + 1 });
            }

            if (ProposedLocationIsValid(knight.CurrentLocation.X - 1, knight.CurrentLocation.Y + 2, knight.Color))
            {
                locations.Add(new Location { X = knight.CurrentLocation.X - 1, Y = knight.CurrentLocation.Y + 2 });
            }

            if (ProposedLocationIsValid(knight.CurrentLocation.X - 2, knight.CurrentLocation.Y + 1, knight.Color))
            {
                locations.Add(new Location { X = knight.CurrentLocation.X - 2, Y = knight.CurrentLocation.Y + 1 });
            }

            if (ProposedLocationIsValid(knight.CurrentLocation.X + 1, knight.CurrentLocation.Y - 1, knight.Color))
            {
                locations.Add(new Location { X = knight.CurrentLocation.X + 1, Y = knight.CurrentLocation.Y - 1 });
            }

            if (ProposedLocationIsValid(knight.CurrentLocation.X + 1, knight.CurrentLocation.Y - 2, knight.Color))
            {
                locations.Add(new Location { X = knight.CurrentLocation.X + 1, Y = knight.CurrentLocation.Y - 2 });
            }

            if (ProposedLocationIsValid(knight.CurrentLocation.X + 2, knight.CurrentLocation.Y - 2, knight.Color))
            {
                locations.Add(new Location { X = knight.CurrentLocation.X + 2, Y = knight.CurrentLocation.Y - 2 });
            }

            if (ProposedLocationIsValid(knight.CurrentLocation.X - 2, knight.CurrentLocation.Y - 1, knight.Color))
            {
                locations.Add(new Location { X = knight.CurrentLocation.X - 2, Y = knight.CurrentLocation.Y - 1 });
            }

            return locations;
        }

        HashSet<Location> GetAvailablePawnLocations(ChessPiece chessPiece)
        {
            int direction = chessPiece.Color == Color.White ? 1 : -1;

            HashSet<Location> locations = new HashSet<Location>();

            Location moveUpLocation = new Location { X = chessPiece.CurrentLocation.X, Y = chessPiece.CurrentLocation.Y + 1 * direction };

            Location moveUpTwoLocations = new Location { X = chessPiece.CurrentLocation.X, Y = chessPiece.CurrentLocation.Y + 2 * direction };

            Location attackRight = new Location { X = chessPiece.CurrentLocation.X + 1 * direction, Y = chessPiece.CurrentLocation.Y + 1 * direction };

            Location attackLeft = new Location { X = chessPiece.CurrentLocation.X - 1 * direction, Y = chessPiece.CurrentLocation.Y + 1 * direction };


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
                locations.Add(new Location { X = x, Y = chessPiece.CurrentLocation.Y });

                x++;

                if(chessPiece is King || !LocationIsEmpty(x, chessPiece.CurrentLocation.Y))
                {
                    break;
                }
            }


            x = chessPiece.CurrentLocation.X - 1;

            while (ProposedLocationIsValid(x, chessPiece.CurrentLocation.Y, chessPiece.Color))
            {
                locations.Add(new Location { X = x, Y = chessPiece.CurrentLocation.Y });

                x--;

                if (chessPiece is King || !LocationIsEmpty(x, chessPiece.CurrentLocation.Y))
                {
                    break;
                }
            }

            return locations;
        }

        HashSet<Location> GetVerticalLocations(ChessPiece chessPiece)
        {
            HashSet<Location> locations = new HashSet<Location>();

            int y = chessPiece.CurrentLocation.Y + 1 ;

            while (ProposedLocationIsValid(chessPiece.CurrentLocation.X, y, chessPiece.Color))
            {
                locations.Add(new Location { X = chessPiece.CurrentLocation.X, Y = y });

                y++;

                if (chessPiece is King || !LocationIsEmpty(chessPiece.CurrentLocation.X, y))
                {
                    break;
                }
            }

            y = chessPiece.CurrentLocation.Y - 1;

            while (ProposedLocationIsValid(chessPiece.CurrentLocation.X, y, chessPiece.Color))
            {
                locations.Add(new Location { X = chessPiece.CurrentLocation.X, Y = y });

                y--;

                if (chessPiece is King || !LocationIsEmpty(chessPiece.CurrentLocation.X, y))
                {
                    break;
                }
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
                locations.Add(new Location { X = x, Y = y });

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
