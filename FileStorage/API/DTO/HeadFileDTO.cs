namespace API.DTO;

public class HeadFileDTO
{
    public string Name { get; set; }
    public long Size { get; set; }
    public bool Exists { get; set; }
    public DateTime LastModified { get; set; }
}