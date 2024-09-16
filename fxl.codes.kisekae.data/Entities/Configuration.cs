namespace fxl.codes.kisekae.data.Entities
{
    public class Configuration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Kisekae Kisekae { get; set; }
        public List<CelConfig> Cels { get; set; } = new();
        public List<Action> Actions { get; set; } = new();

        public string Data { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string BackgroundColorHex { get; set; }
    }
}