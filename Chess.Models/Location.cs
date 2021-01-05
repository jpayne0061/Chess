namespace Chess.Models
{
    public class Location
    {
        public Location()
        {

        }

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
