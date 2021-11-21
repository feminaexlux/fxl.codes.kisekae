namespace fxl.codes.kisekae.Models
{
    public class CoordinateModel
    {
        internal CoordinateModel(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}