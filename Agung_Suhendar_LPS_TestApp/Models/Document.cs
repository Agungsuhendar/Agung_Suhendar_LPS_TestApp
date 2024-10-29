namespace Agung_Suhendar_LPS_TestApp.Models
{
    public class Document
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public byte[]? ContentFile { get; set; }
        public DateTime? UploadedDate { get; set; }
    }
}
