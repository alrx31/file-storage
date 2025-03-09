using API.DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public interface IFilesService
{
    Task<List<string>> GetAll();
    Task<FileStreamResult> GetFile(string path);
    Task Upload(UploadFileDTO file);
    Task UploadWithRewrite(UploadFileDTO file);
    Task<HeadFileDTO> HeadInformation(string path);
    Task Delete(string path);
    Task CopyFileTo(CopyFileDTO model, string path);
}