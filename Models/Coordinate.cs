namespace fxl.codes.kisekae.Models
{
    public class Coordinate
    {
        public Coordinate(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}