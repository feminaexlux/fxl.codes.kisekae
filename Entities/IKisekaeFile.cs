namespace fxl.codes.kisekae.Entities
{
    public interface IKisekaeFile
    {
        int Id { get; set; }
        int KisekaeId { get; set; }
        string Filename { get; set; }
    }

    public interface IKisekaeParseable
    {
        byte[] Data { get; set; }
    }
}