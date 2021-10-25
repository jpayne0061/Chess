namespace Chess.Models
{
    public struct Location
    {
        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }

        public bool Equals(Location location)
        {
            return location.X == X && location.Y == Y;
        }

        public bool Equals(int x, int y)
        {
            return X == x && Y == y;
        }
    }
}
