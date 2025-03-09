namespace API.DTO;

public class UploadFileDTO
{
    public IFormFile file { get; set; }
    public string path { get; set; }
}