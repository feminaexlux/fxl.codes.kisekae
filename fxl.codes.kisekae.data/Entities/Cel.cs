namespace fxl.codes.kisekae.data.Entities
{
    public class Cel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public Kisekae Kisekae { get; set; }
    }
}