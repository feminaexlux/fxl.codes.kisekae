namespace fxl.codes.kisekae.Entities
{
    public class ConfigurationDto : IKisekaeFile
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string Data { get; set; }
        public int KisekaeId { get; set; }
    }
}